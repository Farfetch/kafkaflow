namespace KafkaFlow
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using KafkaFlow.Configuration;
    using KafkaFlow.Consumers;
    using KafkaFlow.Producers;

    internal class KafkaBus : IKafkaBus
    {
        private readonly IDependencyResolver dependencyResolver;
        private readonly KafkaConfiguration configuration;
        private readonly IConsumerManager consumerManager;
        private readonly ILogHandler logHandler;
        private readonly IList<IKafkaConsumer> consumers = new List<IKafkaConsumer>();

        public KafkaBus(
            IDependencyResolver dependencyResolver,
            IConsumerManager consumerManager,
            IProducerAccessor accessor,
            ILogHandler logHandler,
            KafkaConfiguration configuration)
        {
            this.dependencyResolver = dependencyResolver;
            this.configuration = configuration;
            this.consumerManager = consumerManager;
            this.logHandler = logHandler;
            this.Producers = accessor;
        }

        public IConsumerAccessor Consumers => this.consumerManager;

        public IProducerAccessor Producers { get; }

        public async Task StartAsync(CancellationToken stopCancellationToken = default)
        {
            foreach (var consumerConfiguration in this.configuration.Clusters.SelectMany(cl => cl.Consumers))
            {
                var dependencyScope = this.dependencyResolver.CreateScope();

                var middlewares = consumerConfiguration.MiddlewareConfiguration.Factories
                    .Select(factory => factory(dependencyScope.Resolver))
                    .ToList();

                var consumerWorkerPool = new ConsumerWorkerPool(
                    dependencyScope.Resolver,
                    this.logHandler,
                    new MiddlewareExecutor(middlewares),
                    consumerConfiguration.DistributionStrategyFactory);

                var consumer = new KafkaConsumer(
                    consumerConfiguration,
                    this.consumerManager,
                    this.logHandler,
                    consumerWorkerPool,
                    this.dependencyResolver,
                    stopCancellationToken);

                this.consumers.Add(consumer);

                await consumer.StartAsync().ConfigureAwait(false);
            }
        }

        public Task StopAsync()
        {
            return Task.WhenAll(this.consumers.Select(x => x.StopAsync()));
        }
    }
}
