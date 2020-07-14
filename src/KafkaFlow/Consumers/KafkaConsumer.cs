namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using KafkaFlow.Configuration;

    internal class KafkaConsumer
    {
        private readonly ConsumerConfiguration configuration;
        private readonly IConsumerManager consumerManager;
        private readonly ILogHandler logHandler;
        private readonly IConsumerWorkerPool consumerWorkerPool;

        private readonly ConsumerBuilder<byte[], byte[]> consumerBuilder;

        private CancellationTokenSource cancellationTokenSource;
        private Task backgroundTask;

        public KafkaConsumer(
            ConsumerConfiguration configuration,
            IConsumerManager consumerManager,
            ILogHandler logHandler,
            IConsumerWorkerPool consumerWorkerPool)
        {
            this.configuration = configuration;
            this.consumerManager = consumerManager;
            this.logHandler = logHandler;
            this.consumerWorkerPool = consumerWorkerPool;

            var kafkaConfig = configuration.GetKafkaConfig();

            this.consumerBuilder = new ConsumerBuilder<byte[], byte[]>(kafkaConfig);

            this.consumerBuilder.SetPartitionsAssignedHandler((consumer, partitions) => this.OnPartitionAssigned(consumer, partitions));
            this.consumerBuilder.SetPartitionsRevokedHandler((consumer, partitions) => this.OnPartitionRevoked(partitions));
        }

        private void OnPartitionRevoked(IReadOnlyCollection<TopicPartitionOffset> topicPartitions)
        {
            this.logHandler.Warning(
                "Partitions revoked",
                new
                {
                    this.configuration.GroupId,
                    Topics = topicPartitions
                        .GroupBy(x => x.Topic)
                        .Select(
                            x => new
                            {
                                x.First().Topic,
                                PartitionsCount = x.Count(),
                                Partitions = x.Select(y => y.Partition.Value)
                            })
                });

            this.consumerWorkerPool.StopAsync().GetAwaiter().GetResult();
        }

        private void OnPartitionAssigned(IConsumer<byte[], byte[]> consumer, IReadOnlyCollection<TopicPartition> topicPartitions)
        {
            this.logHandler.Info(
                "Partitions assigned",
                new
                {
                    this.configuration.GroupId,
                    Topics = topicPartitions
                        .GroupBy(x => x.Topic)
                        .Select(
                            x => new
                            {
                                x.First().Topic,
                                PartitionsCount = x.Count(),
                                Partitions = x.Select(y => y.Partition.Value)
                            })
                });

            this.consumerWorkerPool
                .StartAsync(
                    consumer,
                    topicPartitions,
                    this.cancellationTokenSource.Token)
                .GetAwaiter()
                .GetResult();
        }

        public Task StartAsync(CancellationToken stopCancellationToken = default)
        {
            this.cancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(stopCancellationToken);

            this.CreateBackgroundTask();

            return Task.CompletedTask;
        }

        public async Task StopAsync()
        {
            await this.consumerWorkerPool.StopAsync().ConfigureAwait(false);

            if (this.cancellationTokenSource.Token.CanBeCanceled)
            {
                this.cancellationTokenSource.Cancel();
            }

            await this.backgroundTask.ConfigureAwait(false);
            this.backgroundTask.Dispose();
        }

        private void CreateBackgroundTask()
        {
            var consumer = this.consumerBuilder.Build();

            this.consumerManager.AddOrUpdate(
                new MessageConsumer(
                    consumer,
                    this.configuration.ConsumerName,
                    this.configuration.GroupId));

            consumer.Subscribe(this.configuration.Topics);

            this.backgroundTask = Task.Factory.StartNew(
                async () =>
                {
                    using (consumer)
                    {
                        while (!this.cancellationTokenSource.IsCancellationRequested)
                        {
                            try
                            {
                                var message = consumer.Consume(this.cancellationTokenSource.Token);

                                await this.consumerWorkerPool
                                    .EnqueueAsync(message, this.cancellationTokenSource.Token)
                                    .ConfigureAwait(false);
                            }
                            catch (OperationCanceledException)
                            {
                                // Ignores the exception
                            }
                            catch (KafkaException ex) when (ex.Error.IsFatal)
                            {
                                this.logHandler.Error(
                                    "Kafka fatal error occurred. Trying to restart in 5 seconds",
                                    ex,
                                    null);

                                await this.consumerWorkerPool.StopAsync().ConfigureAwait(false);
                                _ = Task.Delay(5000).ContinueWith(t => this.CreateBackgroundTask());

                                break;
                            }
                            catch (Exception ex)
                            {
                                this.logHandler.Warning(
                                    "Error consuming message from Kafka",
                                    ex);
                            }
                        }

                        consumer.Close();
                    }
                },
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }
    }
}
