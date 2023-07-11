namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using KafkaFlow.Configuration;

    internal class ConsumerManager : IConsumerManager, IDisposable
    {
        private readonly IOffsetCommitter offsetCommitter;
        private readonly IDependencyResolver dependencyResolver;
        private readonly ILogHandler logHandler;
        private readonly Timer evaluateWorkersCountTimer;

        public ConsumerManager(
            IConsumer consumer,
            IConsumerWorkerPool consumerWorkerPool,
            IWorkerPoolFeeder feeder,
            IOffsetCommitter offsetCommitter,
            IDependencyResolver dependencyResolver,
            ILogHandler logHandler)
        {
            this.offsetCommitter = offsetCommitter;
            this.dependencyResolver = dependencyResolver;
            this.logHandler = logHandler;
            this.Consumer = consumer;
            this.WorkerPool = consumerWorkerPool;
            this.Feeder = feeder;

            this.evaluateWorkersCountTimer = new Timer(
                state => _ = this.EvaluateWorkersCountAsync(),
                null,
                Timeout.Infinite,
                Timeout.Infinite);

            this.Consumer.OnPartitionsAssigned((_, _, partitions) => this.OnPartitionAssigned(partitions));
            this.Consumer.OnPartitionsRevoked((_, _, partitions) => this.OnPartitionRevoked(partitions));
        }

        public IWorkerPoolFeeder Feeder { get; }

        public IConsumerWorkerPool WorkerPool { get; }

        public IConsumer Consumer { get; }

        public Task StartAsync()
        {
            this.Feeder.Start();

            this.StartEvaluateWorkerCountTimer();

            return Task.CompletedTask;
        }

        public async Task StopAsync()
        {
            this.StopEvaluateWorkerCountTimer();

            await this.Feeder.StopAsync().ConfigureAwait(false);
            await this.WorkerPool.StopAsync().ConfigureAwait(false);

            this.offsetCommitter.Dispose();

            this.Consumer.Dispose();
        }

        public void Dispose()
        {
            this.evaluateWorkersCountTimer.Dispose();
        }

        private void StopEvaluateWorkerCountTimer() => this.evaluateWorkersCountTimer.Change(Timeout.Infinite, Timeout.Infinite);

        private void StartEvaluateWorkerCountTimer() => this.evaluateWorkersCountTimer.Change(
            this.Consumer.Configuration.WorkersCountEvaluationInterval,
            this.Consumer.Configuration.WorkersCountEvaluationInterval);

        private async Task EvaluateWorkersCountAsync()
        {
            var newWorkersCount = await this.CalculateWorkersCount(this.Consumer.Assignment);

            if (newWorkersCount == this.WorkerPool.CurrentWorkersCount)
            {
                return;
            }

            await this.ChangeWorkersCountAsync(newWorkersCount);
        }

        private async Task ChangeWorkersCountAsync(int workersCount)
        {
            try
            {
                this.StopEvaluateWorkerCountTimer();

                await this.Feeder.StopAsync();
                await this.WorkerPool.StopAsync();

                await this.WorkerPool.StartAsync(this.Consumer.Assignment, workersCount);
                this.Feeder.Start();

                this.StartEvaluateWorkerCountTimer();
            }
            catch (Exception e)
            {
                this.logHandler.Error("Error changing workers count", e, null);
            }
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

            var workersCount = this.CalculateWorkersCount(partitions).GetAwaiter().GetResult();

            this.WorkerPool
                .StartAsync(partitions, workersCount)
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

        private async Task<int> CalculateWorkersCount(IEnumerable<TopicPartition> partitions)
        {
            try
            {
                return await this.Consumer.Configuration.WorkersCountCalculator(
                    new WorkersCountContext(
                        this.Consumer.Configuration.ConsumerName,
                        this.Consumer.Configuration.GroupId,
                        partitions
                            .GroupBy(
                                x => x.Topic,
                                (topic, grouped) => new TopicPartitions(
                                    topic,
                                    grouped
                                        .Select(x => x.Partition.Value)
                                        .ToList()))
                            .ToList()),
                    this.dependencyResolver);
            }
            catch (Exception e)
            {
                this.logHandler.Error("Error calculating new workers count, using one worker as fallback", e, null);

                return 1;
            }
        }
    }
}
