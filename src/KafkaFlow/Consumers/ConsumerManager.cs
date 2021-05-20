namespace KafkaFlow.Consumers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;

    internal class ConsumerManager : IConsumerManager
    {
        private readonly ILogHandler logHandler;

        private CancellationTokenSource stopCancellationTokenSource;

        public ConsumerManager(
            IConsumer consumer,
            IConsumerWorkerPool consumerWorkerPool,
            IWorkerPoolFeeder feeder,
            ILogHandler logHandler)
        {
            this.logHandler = logHandler;
            this.Consumer = consumer;
            this.WorkerPool = consumerWorkerPool;
            this.Feeder = feeder;

            this.Consumer.OnPartitionsAssigned((resolver, _, partitions) => this.OnPartitionAssigned(partitions));
            this.Consumer.OnPartitionsRevoked((_, partitions) => this.OnPartitionRevoked(partitions));
        }

        public IWorkerPoolFeeder Feeder { get; }

        public IConsumerWorkerPool WorkerPool { get; }

        public IConsumer Consumer { get; }

        public Task StartAsync()
        {
            this.stopCancellationTokenSource = new CancellationTokenSource();

            this.Feeder.Start();

            return Task.CompletedTask;
        }

        public async Task StopAsync()
        {
            if (this.stopCancellationTokenSource != null && this.stopCancellationTokenSource.Token.CanBeCanceled)
            {
                this.stopCancellationTokenSource.Cancel();
            }

            await this.Feeder.StopAsync().ConfigureAwait(false);
            await this.WorkerPool.StopAsync().ConfigureAwait(false);

            this.Consumer.Dispose();
        }

        private void OnPartitionRevoked(IEnumerable<TopicPartitionOffset> topicPartitions)
        {
            this.logHandler.Warning(
                "Partitions revoked",
                this.GetConsumerLogInfo(topicPartitions.Select(x => x.TopicPartition)));

            this.WorkerPool.StopAsync().GetAwaiter().GetResult();
        }

        private void OnPartitionAssigned(IReadOnlyCollection<TopicPartition> partitions)
        {
            this.logHandler.Info(
                "Partitions assigned",
                this.GetConsumerLogInfo(partitions));

            this.WorkerPool
                .StartAsync(partitions)
                .GetAwaiter()
                .GetResult();
        }

        private object GetConsumerLogInfo(IEnumerable<TopicPartition> partitions) => new
        {
            this.Consumer.Configuration.GroupId,
            this.Consumer.Configuration.ConsumerName,
            Topics = partitions
                .GroupBy(x => x.Topic)
                .Select(
                    x => new
                    {
                        x.First().Topic,
                        PartitionsCount = x.Count(),
                        Partitions = x.Select(y => y.Partition.Value),
                    }),
        };
    }
}
