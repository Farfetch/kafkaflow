namespace KafkaFlow
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using KafkaFlow.Clusters;
    using KafkaFlow.Configuration;
    using KafkaFlow.Consumers;
    using KafkaFlow.Producers;

    internal class KafkaBus : IKafkaBus
    {
        private readonly IDependencyResolver dependencyResolver;
        private readonly KafkaConfiguration configuration;
        private readonly IConsumerManagerFactory consumerManagerFactory;
        private readonly IClusterManagerAccessor clusterManagerAccessor;

        private readonly List<IConsumerManager> consumerManagers = new();

        public KafkaBus(
            IDependencyResolver dependencyResolver,
            KafkaConfiguration configuration,
            IConsumerManagerFactory consumerManagerFactory,
            IConsumerAccessor consumers,
            IProducerAccessor producers,
            IClusterManagerAccessor clusterManagerAccessor)
        {
            this.dependencyResolver = dependencyResolver;
            this.configuration = configuration;
            this.consumerManagerFactory = consumerManagerFactory;
            this.clusterManagerAccessor = clusterManagerAccessor;
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
                await this.CreateMissingClusterTopics(cluster);

                foreach (var consumerConfiguration in cluster.Consumers)
                {
                    var dependencyScope = this.dependencyResolver.CreateScope();

                    var consumerManager =
                        this.consumerManagerFactory.Create(consumerConfiguration, dependencyScope.Resolver);

                    this.consumerManagers.Add(consumerManager);
                    this.Consumers.Add(
                        new MessageConsumer(
                            consumerManager,
                            dependencyScope.Resolver.Resolve<ILogHandler>()));

                    if (consumerConfiguration.InitialState == ConsumerInitialState.Running)
                    {
                        await consumerManager.StartAsync().ConfigureAwait(false);
                    }
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

        private async Task CreateMissingClusterTopics(ClusterConfiguration cluster)
        {
            if (cluster.TopicsToCreateIfNotExist is { Count: 0 })
            {
                return;
            }

            await this.clusterManagerAccessor[cluster.Name].CreateIfNotExistsAsync(
                cluster.TopicsToCreateIfNotExist);
        }
    }
}
