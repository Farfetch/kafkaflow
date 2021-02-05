namespace KafkaFlow.Consumers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;

    internal class ConsumerWorkerPool : IConsumerWorkerPool
    {
        private readonly IDependencyResolver dependencyResolver;
        private readonly ILogHandler logHandler;
        private readonly IMiddlewareExecutor middlewareExecutor;
        private readonly Factory<IDistributionStrategy> distributionStrategyFactory;

        private List<IConsumerWorker> workers = new List<IConsumerWorker>();

        private IDistributionStrategy distributionStrategy;
        private OffsetManager offsetManager;

        public ConsumerWorkerPool(
            IDependencyResolver dependencyResolver,
            ILogHandler logHandler,
            IMiddlewareExecutor middlewareExecutor,
            Factory<IDistributionStrategy> distributionStrategyFactory)
        {
            this.dependencyResolver = dependencyResolver;
            this.logHandler = logHandler;
            this.middlewareExecutor = middlewareExecutor;
            this.distributionStrategyFactory = distributionStrategyFactory;
        }

        public async Task StartAsync(
            IKafkaConsumer consumer,
            IEnumerable<TopicPartition> partitions,
            CancellationToken stopCancellationToken = default)
        {
            this.offsetManager = new OffsetManager(
                new OffsetCommitter(
                    consumer,
                    consumer.Configuration.AutoCommitInterval,
                    this.logHandler),
                partitions);

            await Task.WhenAll(
                    Enumerable
                        .Range(0, consumer.Configuration.WorkerCount)
                        .Select(
                            workerId =>
                            {
                                var worker = new ConsumerWorker(
                                    consumer,
                                    workerId,
                                    this.offsetManager,
                                    this.logHandler,
                                    this.middlewareExecutor);

                                this.workers.Add(worker);

                                return worker.StartAsync(stopCancellationToken);
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

        public async Task EnqueueAsync(ConsumeResult<byte[], byte[]> message, CancellationToken stopCancellationToken = default)
        {
            var worker = (IConsumerWorker) await this.distributionStrategy
                .GetWorkerAsync(message.Message.Key, stopCancellationToken)
                .ConfigureAwait(false);

            if (worker == null)
            {
                return;
            }

            this.offsetManager.AddOffset(message.TopicPartitionOffset);

            await worker
                .EnqueueAsync(message, stopCancellationToken)
                .ConfigureAwait(false);
        }
    }
}
