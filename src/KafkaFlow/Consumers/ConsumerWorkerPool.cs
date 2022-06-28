namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using KafkaFlow.Configuration;

    internal class ConsumerWorkerPool : IConsumerWorkerPool
    {
        private readonly IConsumer consumer;
        private readonly IDependencyResolver dependencyResolver;
        private readonly IMiddlewareExecutor middlewareExecutor;
        private readonly ILogHandler logHandler;
        private readonly Factory<IDistributionStrategy> distributionStrategyFactory;
        private readonly IReadOnlyList<(Action<IDependencyResolver, IEnumerable<TopicPartitionOffset>> handler, TimeSpan interval)> pendingOffsetsHandlers;

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
            this.pendingOffsetsHandlers = consumerConfiguration.PendingOffsetsHandlers;
            this.distributionStrategyFactory = consumerConfiguration.DistributionStrategyFactory;
        }

        public async Task StartAsync(IEnumerable<TopicPartition> partitions)
        {
            this.offsetManager = new OffsetManager(
                new OffsetCommitter(
                    this.consumer,
                    this.dependencyResolver,
                    this.pendingOffsetsHandlers,
                    this.logHandler),
                partitions);

            await Task.WhenAll(
                    Enumerable
                        .Range(0, this.consumer.Configuration.WorkersCount)
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
        }

        public async Task StopAsync()
        {
            var currentWorkers = this.workers;
            this.workers = new List<IConsumerWorker>();

            await Task.WhenAll(currentWorkers.Select(x => x.StopAsync())).ConfigureAwait(false);

            this.offsetManager?.Dispose();
            this.offsetManager = null;
        }

        public async Task EnqueueAsync(ConsumeResult<byte[], byte[]> message, CancellationToken stopCancellationToken)
        {
            var localOffsetManager = this.offsetManager;
            var worker = (IConsumerWorker) await this.distributionStrategy
                .GetWorkerAsync(message.Message.Key, stopCancellationToken)
                .ConfigureAwait(false);

            if (worker is null || localOffsetManager is null)
            {
                return;
            }

            localOffsetManager.Enqueue(message.TopicPartitionOffset);

            await worker
                .EnqueueAsync(message, stopCancellationToken)
                .ConfigureAwait(false);
        }
    }
}
