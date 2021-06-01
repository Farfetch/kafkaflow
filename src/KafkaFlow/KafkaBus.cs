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
        private readonly IConsumerManagerFactory consumerManagerFactory;

        private readonly List<IConsumerManager> consumerManagers = new();

        public KafkaBus(
            IDependencyResolver dependencyResolver,
            KafkaConfiguration configuration,
            IConsumerManagerFactory consumerManagerFactory,
            IConsumerAccessor consumers,
            IProducerAccessor producers)
        {
            this.dependencyResolver = dependencyResolver;
            this.configuration = configuration;
            this.consumerManagerFactory = consumerManagerFactory;
            this.Consumers = consumers;
            this.Producers = producers;
        }

        public IConsumerAccessor Consumers { get; }

        public IProducerAccessor Producers { get; }

        public async Task StartAsync(CancellationToken stopCancellationToken = default)
        {
            var stopTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stopCancellationToken);

            stopTokenSource.Token.Register(() => this.StopAsync().GetAwaiter().GetResult());

            foreach (var cluster in this.configuration.Clusters)
            {
                foreach (var consumerConfiguration in cluster.Consumers)
                {
                    var dependencyScope = this.dependencyResolver.CreateScope();

                    var consumerManager = this.consumerManagerFactory.Create(consumerConfiguration, dependencyScope.Resolver);

                    this.consumerManagers.Add(consumerManager);
                    this.Consumers.Add(
                        new MessageConsumer(
                            consumerManager,
                            dependencyScope.Resolver.Resolve<ILogHandler>()));

                    await consumerManager.StartAsync().ConfigureAwait(false);
                }

                cluster.OnStartedHandler(this.dependencyResolver);
            }
        }

        public Task StopAsync()
        {
            foreach (var cluster in this.configuration.Clusters)
            {
                cluster.OnStoppingHandler(this.dependencyResolver);
            }

            return Task.WhenAll(this.consumerManagers.Select(x => x.StopAsync()));
        }
    }
}
