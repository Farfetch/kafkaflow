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
        private readonly CancellationTokenSource stopCancellationTokenSource = new();

        public KafkaBus(
            IDependencyResolver dependencyResolver,
            KafkaConfiguration configuration,
            IConsumerManagerFactory consumerManagerFactory,
            IClusterAccessor clusters,
            IConsumerAccessor consumers,
            IProducerAccessor producers)
        {
            this.dependencyResolver = dependencyResolver;
            this.configuration = configuration;
            this.consumerManagerFactory = consumerManagerFactory;
            this.Clusters = clusters;
            this.Consumers = consumers;
            this.Producers = producers;

            this.stopCancellationTokenSource.Token.Register(() => this.InternalStopAsync().GetAwaiter().GetResult());
        }

        public IClusterAccessor Clusters { get; }

        public IConsumerAccessor Consumers { get; }

        public IProducerAccessor Producers { get; }

        public CancellationToken BusStopping => this.stopCancellationTokenSource.Token;

        public async Task StartAsync(CancellationToken stopCancellationToken = default)
        {
            stopCancellationToken.Register(() => this.stopCancellationTokenSource.Cancel());

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
            this.stopCancellationTokenSource.Cancel();
            this.stopCancellationTokenSource.Token.WaitHandle.WaitOne();
            return Task.CompletedTask;
        }

        private Task InternalStopAsync()
        {
            foreach (var cluster in this.configuration.Clusters)
            {
                cluster.OnStoppingHandler(this.dependencyResolver);
            }

            return Task.WhenAll(this.consumerManagers.Select(x => x.StopAsync()));
        }
    }
}
