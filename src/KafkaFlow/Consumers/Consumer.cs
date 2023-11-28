using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KafkaFlow.Configuration;

namespace KafkaFlow.Consumers
{
    internal class Consumer : IConsumer
    {
        private readonly IDependencyResolver _dependencyResolver;
        private readonly ILogHandler _logHandler;

        private readonly List<Action<IDependencyResolver, Confluent.Kafka.IConsumer<byte[], byte[]>, List<Confluent.Kafka.TopicPartition>>>
            _partitionsAssignedHandlers = new();

        private readonly List<Action<IDependencyResolver, Confluent.Kafka.IConsumer<byte[], byte[]>, List<Confluent.Kafka.TopicPartitionOffset>>>
            _partitionsRevokedHandlers = new();

        private readonly List<Action<Confluent.Kafka.IConsumer<byte[], byte[]>, Confluent.Kafka.Error>> _errorsHandlers = new();
        private readonly List<Action<Confluent.Kafka.IConsumer<byte[], byte[]>, string>> _statisticsHandlers = new();
        private readonly ConcurrentDictionary<Confluent.Kafka.TopicPartition, long> _currentPartitionsOffsets = new();
        private readonly ConsumerFlowManager _flowManager;

        private Confluent.Kafka.IConsumer<byte[], byte[]> _consumer;

        public Consumer(
            IConsumerConfiguration configuration,
            IDependencyResolver dependencyResolver,
            ILogHandler logHandler)
        {
            _dependencyResolver = dependencyResolver;
            _logHandler = logHandler;
            this.Configuration = configuration;
            _flowManager = new ConsumerFlowManager(
                this,
                _logHandler);

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

            var middlewareContext = _dependencyResolver.Resolve<ConsumerMiddlewareContext>();

            middlewareContext.Worker = null;
            middlewareContext.Consumer = this;

            this.RegisterLogErrorHandler();
        }

        public IConsumerConfiguration Configuration { get; }

        public IReadOnlyList<string> Subscription { get; private set; } = new List<string>();

        public IReadOnlyList<Confluent.Kafka.TopicPartition> Assignment { get; private set; } = new List<Confluent.Kafka.TopicPartition>();

        public IConsumerFlowManager FlowManager => _flowManager;

        public string MemberId => _consumer?.MemberId;

        public string ClientInstanceName => _consumer?.Name;

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

        public void OnPartitionsAssigned(Action<IDependencyResolver, Confluent.Kafka.IConsumer<byte[], byte[]>, List<Confluent.Kafka.TopicPartition>> handler) =>
            _partitionsAssignedHandlers.Add(handler);

        public void OnPartitionsRevoked(Action<IDependencyResolver, Confluent.Kafka.IConsumer<byte[], byte[]>, List<Confluent.Kafka.TopicPartitionOffset>> handler) =>
            _partitionsRevokedHandlers.Add(handler);

        public void OnError(Action<Confluent.Kafka.IConsumer<byte[], byte[]>, Confluent.Kafka.Error> handler) =>
            _errorsHandlers.Add(handler);

        public void OnStatistics(Action<Confluent.Kafka.IConsumer<byte[], byte[]>, string> handler) =>
            _statisticsHandlers.Add(handler);

        public Confluent.Kafka.Offset GetPosition(Confluent.Kafka.TopicPartition topicPartition) =>
            _consumer.Position(topicPartition);

        public Confluent.Kafka.WatermarkOffsets GetWatermarkOffsets(Confluent.Kafka.TopicPartition topicPartition) =>
            _consumer.GetWatermarkOffsets(topicPartition);

        public Confluent.Kafka.WatermarkOffsets QueryWatermarkOffsets(Confluent.Kafka.TopicPartition topicPartition, TimeSpan timeout) =>
            _consumer.QueryWatermarkOffsets(topicPartition, timeout);

        public List<Confluent.Kafka.TopicPartitionOffset> OffsetsForTimes(
            IEnumerable<Confluent.Kafka.TopicPartitionTimestamp> topicPartitions,
            TimeSpan timeout) =>
            _consumer.OffsetsForTimes(topicPartitions, timeout);

        public IEnumerable<TopicPartitionLag> GetTopicPartitionsLag()
        {
            return this.Assignment.Select(
                tp =>
                {
                    var offset = Math.Max(0, _currentPartitionsOffsets.GetOrAdd(tp, _ => this.GetPosition(tp)));
                    var offsetEnd = Math.Max(0, this.GetWatermarkOffsets(tp).High.Value);

                    return new TopicPartitionLag(tp.Topic, tp.Partition.Value, offset == 0 ? 0 : offsetEnd - offset);
                });
        }

        public void Commit(IReadOnlyCollection<Confluent.Kafka.TopicPartitionOffset> offsets)
        {
            var validOffsets = offsets
                .Where(x => x.Offset.Value >= 0)
                .ToList();

            if (!validOffsets.Any())
            {
                return;
            }

            _consumer.Commit(validOffsets);

            foreach (var offset in validOffsets)
            {
                _currentPartitionsOffsets[offset.TopicPartition] = offset.Offset.Value;
            }
        }

        public async ValueTask<Confluent.Kafka.ConsumeResult<byte[], byte[]>> ConsumeAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    this.EnsureConsumer();
                    await _flowManager.BlockHeartbeat(cancellationToken);
                    return _consumer.Consume(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Confluent.Kafka.KafkaException ex) when (ex.Error.IsFatal)
                {
                    _logHandler.Error(
                        "Kafka Consumer fatal error occurred. Recreating consumer in 5 seconds",
                        ex,
                        null);

                    this.InvalidateConsumer();

                    await Task.Delay(5000, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logHandler.Error("Kafka Consumer Error", ex, null);
                }
                finally
                {
                    _flowManager.ReleaseHeartbeat();
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
                        _logHandler.Error("Kafka Consumer Internal Error", null, errorData);
                    }
                    else
                    {
                        _logHandler.Warning("Kafka Consumer Internal Warning", errorData);
                    }
                });
        }

        private void EnsureConsumer()
        {
            if (_consumer != null)
            {
                return;
            }

            var kafkaConfig = this.Configuration.GetKafkaConfig();

            var consumerBuilder = new Confluent.Kafka.ConsumerBuilder<byte[], byte[]>(kafkaConfig);

            _consumer =
                consumerBuilder
                    .SetPartitionsAssignedHandler(
                        (consumer, partitions) => this.FirePartitionsAssignedHandlers(consumer, partitions))
                    .SetPartitionsRevokedHandler(
                        (consumer, partitions) =>
                        {
                            this.Assignment = new List<Confluent.Kafka.TopicPartition>();
                            this.Subscription = new List<string>();
                            _currentPartitionsOffsets.Clear();
                            _flowManager.Stop();

                            _partitionsRevokedHandlers.ForEach(handler => handler(_dependencyResolver, consumer, partitions));
                        })
                    .SetErrorHandler((consumer, error) => _errorsHandlers.ForEach(x => x(consumer, error)))
                    .SetStatisticsHandler((consumer, statistics) => _statisticsHandlers.ForEach(x => x(consumer, statistics)))
                    .Build();

            if (this.Configuration.Topics.Any())
            {
                _consumer.Subscribe(this.Configuration.Topics);
            }

            if (this.Configuration.ManualAssignPartitions.Any())
            {
                this.ManualAssign(this.Configuration.ManualAssignPartitions);
            }
        }

        private void ManualAssign(IEnumerable<TopicPartitions> topics)
        {
            var partitions = topics
                .SelectMany(topic => topic.Partitions.Select(partition => new Confluent.Kafka.TopicPartition(topic.Name, new Confluent.Kafka.Partition(partition))))
                .ToList();

            _consumer.Assign(partitions);
            this.FirePartitionsAssignedHandlers(_consumer, partitions);
        }

        private void FirePartitionsAssignedHandlers(Confluent.Kafka.IConsumer<byte[], byte[]> consumer, List<Confluent.Kafka.TopicPartition> partitions)
        {
            this.Assignment = partitions;
            this.Subscription = consumer.Subscription;
            _flowManager.Start(consumer);

            _partitionsAssignedHandlers.ForEach(handler => handler(_dependencyResolver, consumer, partitions));
        }

        private void InvalidateConsumer()
        {
            _consumer?.Close();
            _consumer = null;
        }
    }
}
