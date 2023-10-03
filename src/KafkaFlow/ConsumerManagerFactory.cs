namespace KafkaFlow
{
    using KafkaFlow.Configuration;
    using KafkaFlow.Consumers;

    internal class ConsumerManagerFactory : IConsumerManagerFactory
    {
        public IConsumerManager Create(IConsumerConfiguration configuration, IDependencyResolver consumerDependencyResolver)
        {
            var logHandler = consumerDependencyResolver.Resolve<ILogHandler>();

            var consumer = configuration.CustomFactory(
                new Consumer(configuration, consumerDependencyResolver, logHandler),
                consumerDependencyResolver);

            var middlewareExecutor = new MiddlewareExecutor(configuration.MiddlewaresConfigurations);

            var consumerWorkerPool = new ConsumerWorkerPool(
                consumer,
                consumerDependencyResolver,
                middlewareExecutor,
                configuration,
                logHandler);

            consumerWorkerPool.WorkerPoolStopped.Subscribe(() => middlewareExecutor.OnWorkerPoolStopped());

            var feeder = new WorkerPoolFeeder(
                consumer,
                consumerWorkerPool,
                logHandler);

            var consumerManager = new ConsumerManager(
                consumer,
                consumerWorkerPool,
                feeder,
                consumerDependencyResolver,
                logHandler);

            return consumerManager;
        }
    }
}
