namespace KafkaFlow
{
    using KafkaFlow.Configuration;
    using KafkaFlow.Consumers;

    internal class ConsumerManagerFactory : IConsumerManagerFactory
    {
        public IConsumerManager Create(IConsumerConfiguration configuration, IDependencyResolver resolver)
        {
            var logHandler = resolver.Resolve<ILogHandler>();

            var consumer = configuration.CustomFactory(new Consumer(configuration, resolver, logHandler), resolver);

            IOffsetCommitter offsetCommitter = configuration.NoStoreOffsets ?
                new NullOffsetCommitter() :
                new OffsetCommitter(
                    consumer,
                    resolver,
                    configuration.PendingOffsetsHandlers,
                    logHandler);

            var middlewareExecutor = new MiddlewareExecutor(configuration.MiddlewaresConfigurations);

            var consumerWorkerPool = new ConsumerWorkerPool(
                consumer,
                offsetCommitter,
                resolver,
                middlewareExecutor,
                configuration,
                logHandler);

            consumerWorkerPool.WorkerPoolStopped.Subscribe(middlewareExecutor);

            var feeder = new WorkerPoolFeeder(
                consumer,
                consumerWorkerPool,
                logHandler);

            var consumerManager = new ConsumerManager(
                consumer,
                consumerWorkerPool,
                feeder,
                offsetCommitter,
                resolver,
                logHandler);

            return consumerManager;
        }
    }
}
