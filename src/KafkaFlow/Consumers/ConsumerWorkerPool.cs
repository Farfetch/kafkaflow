namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using KafkaFlow.Configuration;

    internal class ConsumerWorkerPool : IConsumerWorkerPool, IDisposable
    {
        private readonly IConsumer consumer;
        private readonly IDependencyResolver dependencyResolver;
        private readonly IMiddlewareExecutor middlewareExecutor;
        private readonly ILogHandler logHandler;
        private readonly Factory<IDistributionStrategy> distributionStrategyFactory;
        private readonly IOffsetCommitter offsetCommitter;

        private TaskCompletionSource<object> startedTaskSource = new();
        private List<IConsumerWorker> workers = new();

        private IDistributionStrategy distributionStrategy;
        private OffsetManager offsetManager;

        public ConsumerWorkerPool(
            IConsumer consumer,
            IDependencyResolver dependencyResolver,
            IMiddlewareExecutor middlewareExecutor,
            IConsumerConfiguration consumerConfiguration,
            ILogHandler logHandler)
        {
            this.consumer = consumer;
            this.dependencyResolver = dependencyResolver;
            this.middlewareExecutor = middlewareExecutor;
            this.logHandler = logHandler;
            this.distributionStrategyFactory = consumerConfiguration.DistributionStrategyFactory;

            this.offsetCommitter = this.consumer.Configuration.NoStoreOffsets ?
                new NullOffsetCommitter() :
                new OffsetCommitter(
                    this.consumer,
                    this.dependencyResolver,
                    consumerConfiguration.PendingOffsetsHandlers,
                    this.logHandler);
        }

        public int CurrentWorkersCount { get; private set; }

        public async Task StartAsync(IReadOnlyCollection<TopicPartition> partitions, int workersCount)
        {
            this.offsetManager = new OffsetManager(this.offsetCommitter, partitions);

            this.CurrentWorkersCount = workersCount;

            await Task.WhenAll(
                    Enumerable
                        .Range(0, this.CurrentWorkersCount)
                        .Select(
                            workerId =>
                            {
                                var worker = new ConsumerWorker(
                                    this.consumer,
                                    this.dependencyResolver,
                                    workerId,
                                    this.offsetManager,
                                    this.middlewareExecutor,
                                    this.logHandler);

                                this.workers.Add(worker);

                                return worker.StartAsync();
                            }))
                .ConfigureAwait(false);

            this.distributionStrategy = this.distributionStrategyFactory(this.dependencyResolver);
            this.distributionStrategy.Init(this.workers.AsReadOnly());

            this.startedTaskSource.TrySetResult(null);
        }

        public async Task StopAsync()
        {
            var currentWorkers = this.workers;
            this.workers = new List<IConsumerWorker>();
            this.startedTaskSource = new();

            await Task.WhenAll(currentWorkers.Select(x => x.StopAsync())).ConfigureAwait(false);

            this.offsetCommitter.CommitProcessedOffsets();

            this.offsetManager = null;
        }

        public async Task EnqueueAsync(ConsumeResult<byte[], byte[]> message, CancellationToken stopCancellationToken)
        {
            await this.startedTaskSource.Task.ConfigureAwait(false);

            var worker = (IConsumerWorker)await this.distributionStrategy
                .GetWorkerAsync(message.Message.Key, stopCancellationToken)
                .ConfigureAwait(false);

            if (worker is null)
            {
                return;
            }

            this.offsetManager?.Enqueue(message.TopicPartitionOffset);

            await worker
                .EnqueueAsync(message, stopCancellationToken)
                .ConfigureAwait(false);
        }

        public void Dispose()
        {
            this.offsetCommitter.Dispose();
        }
    }
}
