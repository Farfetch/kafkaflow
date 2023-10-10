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
        private readonly IDependencyResolver consumerDependencyResolver;
        private readonly IMiddlewareExecutor middlewareExecutor;
        private readonly ILogHandler logHandler;
        private readonly Factory<IWorkerDistributionStrategy> distributionStrategyFactory;
        private readonly IOffsetCommitter offsetCommitter;

        private readonly Event workerPoolStoppedSubject;

        private TaskCompletionSource<object> startedTaskSource = new();
        private List<IConsumerWorker> workers = new();

        private IWorkerDistributionStrategy distributionStrategy;
        private IOffsetManager offsetManager;

        public ConsumerWorkerPool(
            IConsumer consumer,
            IDependencyResolver consumerDependencyResolver,
            IMiddlewareExecutor middlewareExecutor,
            IConsumerConfiguration consumerConfiguration,
            ILogHandler logHandler)
        {
            this.consumer = consumer;
            this.consumerDependencyResolver = consumerDependencyResolver;
            this.middlewareExecutor = middlewareExecutor;
            this.logHandler = logHandler;
            this.distributionStrategyFactory = consumerConfiguration.DistributionStrategyFactory;
            this.workerPoolStoppedSubject = new Event(logHandler);

            this.offsetCommitter = consumer.Configuration.NoStoreOffsets ?
                new NullOffsetCommitter() :
                new OffsetCommitter(
                    consumer,
                    consumerDependencyResolver,
                    logHandler);

            this.offsetCommitter.PendingOffsetsStatisticsHandlers.AddRange(consumer.Configuration.PendingOffsetsStatisticsHandlers);
        }

        public int CurrentWorkersCount { get; private set; }

        public IEvent WorkerPoolStopped => this.workerPoolStoppedSubject;

        public async Task StartAsync(IReadOnlyCollection<TopicPartition> partitions, int workersCount)
        {
            try
            {
                this.offsetManager = this.consumer.Configuration.NoStoreOffsets ?
                    new NullOffsetManager() :
                    new OffsetManager(this.offsetCommitter, partitions);

                await this.offsetCommitter.StartAsync();

                this.CurrentWorkersCount = workersCount;

                await Task.WhenAll(
                        Enumerable
                            .Range(0, this.CurrentWorkersCount)
                            .Select(
                                workerId =>
                                {
                                    var worker = new ConsumerWorker(
                                        this.consumer,
                                        this.consumerDependencyResolver,
                                        workerId,
                                        this.middlewareExecutor,
                                        this.logHandler);

                                    this.workers.Add(worker);

                                    return worker.StartAsync();
                                }))
                    .ConfigureAwait(false);

                this.distributionStrategy = this.distributionStrategyFactory(this.consumerDependencyResolver);
                this.distributionStrategy.Initialize(this.workers.AsReadOnly());

                this.startedTaskSource.TrySetResult(null);
            }
            catch (Exception e)
            {
                this.logHandler.Error(
                    "Error starting WorkerPool",
                    e,
                    new
                    {
                        this.consumer.Configuration.ConsumerName,
                    });
            }
        }

        public async Task StopAsync()
        {
            if (this.workers.Count == 0)
            {
                return;
            }

            var currentWorkers = this.workers;
            this.workers = new List<IConsumerWorker>();
            this.startedTaskSource = new();

            await Task.WhenAll(currentWorkers.Select(x => x.StopAsync())).ConfigureAwait(false);

            await this.offsetManager.WaitContextsCompletionAsync();

            currentWorkers.ForEach(worker => worker.Dispose());

            this.offsetManager = null;

            await this.workerPoolStoppedSubject.FireAsync();

            await this.offsetCommitter.StopAsync();
        }

        public async Task EnqueueAsync(ConsumeResult<byte[], byte[]> message, CancellationToken stopCancellationToken)
        {
            await this.startedTaskSource.Task.ConfigureAwait(false);

            var worker = (IConsumerWorker)await this.distributionStrategy
                .GetWorkerAsync(
                    new WorkerDistributionContext(
                        this.consumer.Configuration.ConsumerName,
                        message.Topic,
                        message.Partition.Value,
                        message.Message.Key,
                        stopCancellationToken))
                .ConfigureAwait(false);

            if (worker is null)
            {
                return;
            }

            var context = this.CreateMessageContext(message, worker);

            await worker
                .EnqueueAsync(context, stopCancellationToken)
                .ConfigureAwait(false);

            this.offsetManager.Enqueue(context.ConsumerContext);
        }

        private MessageContext CreateMessageContext(ConsumeResult<byte[], byte[]> message, IConsumerWorker worker)
        {
            var messageDependencyScope = this.consumerDependencyResolver.CreateScope();

            var context = new MessageContext(
                new Message(message.Message.Key, message.Message.Value),
                new MessageHeaders(message.Message.Headers),
                messageDependencyScope.Resolver,
                new ConsumerContext(
                    this.consumer,
                    this.offsetManager,
                    message,
                    worker,
                    messageDependencyScope,
                    this.consumerDependencyResolver),
                null,
                this.consumer.Configuration.ClusterConfiguration.Brokers);
            return context;
        }
    }
}