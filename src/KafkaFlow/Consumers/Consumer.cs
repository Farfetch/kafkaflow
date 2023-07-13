namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using KafkaFlow.Configuration;

    internal class Consumer : IConsumer
    {
        private readonly IDependencyResolver dependencyResolver;
        private readonly ILogHandler logHandler;

        private readonly List<Action<IDependencyResolver, IConsumer<byte[], byte[]>, List<TopicPartition>>>
            partitionsAssignedHandlers = new();

        private readonly List<Action<IDependencyResolver, IConsumer<byte[], byte[]>, List<TopicPartitionOffset>>>
            partitionsRevokedHandlers = new();

        private readonly List<Action<IConsumer<byte[], byte[]>, Error>> errorsHandlers = new();
        private readonly List<Action<IConsumer<byte[], byte[]>, string>> statisticsHandlers = new();
        private readonly ConcurrentDictionary<TopicPartition, long> currentPartitionsOffsets = new();
        private readonly ConsumerFlowManager flowManager;

        private IConsumer<byte[], byte[]> consumer;

        public Consumer(
            IConsumerConfiguration configuration,
            IDependencyResolver dependencyResolver,
            ILogHandler logHandler)
        {
            this.dependencyResolver = dependencyResolver;
            this.logHandler = logHandler;
            this.Configuration = configuration;
            this.flowManager = new ConsumerFlowManager(
                this,
                this.logHandler);

            foreach (var handler in this.Configuration.StatisticsHandlers)
            {
                this.OnStatistics((_, statistics) => handler(statistics));
            }

            foreach (var handler in this.Configuration.PartitionsAssignedHandlers)
            {
                this.OnPartitionsAssigned((resolver, _, topicPartitions) => handler(resolver, topicPartitions));
            }

            foreach (var handler in this.Configuration.PartitionsRevokedHandlers)
            {
                this.OnPartitionsRevoked((resolver, _, topicPartitions) => handler(resolver, topicPartitions));
            }

            this.RegisterLogErrorHandler();
        }

        public IConsumerConfiguration Configuration { get; }

        public IReadOnlyList<string> Subscription { get; private set; } = new List<string>();

        public IReadOnlyList<TopicPartition> Assignment { get; private set; } = new List<TopicPartition>();

        public IConsumerFlowManager FlowManager => this.flowManager;

        public string MemberId => this.consumer?.MemberId;

        public string ClientInstanceName => this.consumer?.Name;

        public ConsumerStatus Status
        {
            get
            {
                if (this.FlowManager is null || !this.Assignment.Any())
                {
                    return ConsumerStatus.Stopped;
                }

                if (this.FlowManager.PausedPartitions.Count == 0)
                {
                    return ConsumerStatus.Running;
                }

                return this.FlowManager.PausedPartitions.Count == this.Assignment.Count ?
                    ConsumerStatus.Paused :
                    ConsumerStatus.PartiallyRunning;
            }
        }

        public void OnPartitionsAssigned(Action<IDependencyResolver, IConsumer<byte[], byte[]>, List<TopicPartition>> handler) =>
            this.partitionsAssignedHandlers.Add(handler);

        public void OnPartitionsRevoked(Action<IDependencyResolver, IConsumer<byte[], byte[]>, List<TopicPartitionOffset>> handler) =>
            this.partitionsRevokedHandlers.Add(handler);

        public void OnError(Action<IConsumer<byte[], byte[]>, Error> handler) =>
            this.errorsHandlers.Add(handler);

        public void OnStatistics(Action<IConsumer<byte[], byte[]>, string> handler) =>
            this.statisticsHandlers.Add(handler);

        public Offset GetPosition(TopicPartition topicPartition) =>
            this.consumer.Position(topicPartition);

        public WatermarkOffsets GetWatermarkOffsets(TopicPartition topicPartition) =>
            this.consumer.GetWatermarkOffsets(topicPartition);

        public WatermarkOffsets QueryWatermarkOffsets(TopicPartition topicPartition, TimeSpan timeout) =>
            this.consumer.QueryWatermarkOffsets(topicPartition, timeout);

        public List<TopicPartitionOffset> OffsetsForTimes(
            IEnumerable<TopicPartitionTimestamp> topicPartitions,
            TimeSpan timeout) =>
            this.consumer.OffsetsForTimes(topicPartitions, timeout);

        public IEnumerable<TopicPartitionLag> GetTopicPartitionsLag()
        {
            return this.Assignment.Select(
                tp =>
                {
                    var offset = Math.Max(0, this.currentPartitionsOffsets.GetOrAdd(tp, _ => this.GetPosition(tp)));
                    var offsetEnd = Math.Max(0, this.GetWatermarkOffsets(tp).High.Value);

                    return new TopicPartitionLag(tp.Topic, tp.Partition.Value, offset == 0 ? 0 : offsetEnd - offset);
                });
        }

        public void Commit(IReadOnlyCollection<TopicPartitionOffset> offsets)
        {
            var validOffsets = offsets
                .Where(x => x.Offset.Value >= 0)
                .ToList();

            if (!validOffsets.Any())
            {
                return;
            }

            this.consumer.Commit(validOffsets);

            foreach (var offset in validOffsets)
            {
                this.currentPartitionsOffsets[offset.TopicPartition] = offset.Offset.Value;
            }
        }

        public async ValueTask<ConsumeResult<byte[], byte[]>> ConsumeAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    this.EnsureConsumer();
                    await this.flowManager.BlockHeartbeat(cancellationToken);
                    return this.consumer.Consume(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (KafkaException ex) when (ex.Error.IsFatal)
                {
                    this.logHandler.Error(
                        "Kafka Consumer fatal error occurred. Recreating consumer in 5 seconds",
                        ex,
                        null);

                    this.InvalidateConsumer();

                    await Task.Delay(5000, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    this.logHandler.Error("Kafka Consumer Error", ex, null);
                }
                finally
                {
                    this.flowManager.ReleaseHeartbeat();
                }
            }
        }

        public void Dispose() => this.InvalidateConsumer();

        private void RegisterLogErrorHandler()
        {
            this.OnError(
                (_, error) =>
                {
                    var errorData = new
                    {
                        Code = error.Code.ToString(),
                        error.Reason,
                        error.IsBrokerError,
                        error.IsLocalError,
                        error.IsError,
                    };

                    if (error.IsFatal)
                    {
                        this.logHandler.Error("Kafka Consumer Internal Error", null, errorData);
                    }
                    else
                    {
                        this.logHandler.Warning("Kafka Consumer Internal Warning", errorData);
                    }
                });
        }

        private void EnsureConsumer()
        {
            if (this.consumer != null)
            {
                return;
            }

            var kafkaConfig = this.Configuration.GetKafkaConfig();

            var consumerBuilder = new ConsumerBuilder<byte[], byte[]>(kafkaConfig);

            this.consumer =
                consumerBuilder
                    .SetPartitionsAssignedHandler(
                        (consumer, partitions) => this.FirePartitionsAssignedHandlers(consumer, partitions))
                    .SetPartitionsRevokedHandler(
                        (consumer, partitions) =>
                        {
                            this.Assignment = new List<TopicPartition>();
                            this.Subscription = new List<string>();
                            this.currentPartitionsOffsets.Clear();
                            this.flowManager.Stop();

                            this.partitionsRevokedHandlers.ForEach(handler => handler(this.dependencyResolver, consumer, partitions));
                        })
                    .SetErrorHandler((consumer, error) => this.errorsHandlers.ForEach(x => x(consumer, error)))
                    .SetStatisticsHandler((consumer, statistics) => this.statisticsHandlers.ForEach(x => x(consumer, statistics)))
                    .Build();

            if (this.Configuration.Topics.Any())
            {
                this.consumer.Subscribe(this.Configuration.Topics);
            }

            if (this.Configuration.ManualAssignPartitions.Any())
            {
                this.ManualAssign(this.Configuration.ManualAssignPartitions);
            }
        }

        private void ManualAssign(IEnumerable<TopicPartitions> topics)
        {
            var partitions = topics
                .SelectMany(topic => topic.Partitions.Select(partition => new TopicPartition(topic.Name, new Partition(partition))))
                .ToList();

            this.consumer.Assign(partitions);
            this.FirePartitionsAssignedHandlers(this.consumer, partitions);
        }

        private void FirePartitionsAssignedHandlers(IConsumer<byte[], byte[]> consumer, List<TopicPartition> partitions)
        {
            this.Assignment = partitions;
            this.Subscription = consumer.Subscription;
            this.flowManager.Start(consumer);

            this.partitionsAssignedHandlers.ForEach(handler => handler(this.dependencyResolver, consumer, partitions));
        }

        private void InvalidateConsumer()
        {
            this.consumer?.Close();
            this.consumer = null;
        }
    }
}
