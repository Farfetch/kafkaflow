using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KafkaFlow.Clusters;
using KafkaFlow.Configuration;
using KafkaFlow.Consumers;
using KafkaFlow.Producers;

namespace KafkaFlow;

internal class KafkaBus : IKafkaBus
{
    private readonly IDependencyResolver _dependencyResolver;
    private readonly KafkaConfiguration _configuration;
    private readonly IConsumerManagerFactory _consumerManagerFactory;
    private readonly IClusterManagerAccessor _clusterManagerAccessor;

    private readonly List<IConsumerManager> _consumerManagers = new();

    private bool _stopped;

    public KafkaBus(
        IDependencyResolver dependencyResolver,
        KafkaConfiguration configuration,
        IConsumerManagerFactory consumerManagerFactory,
        IConsumerAccessor consumers,
        IProducerAccessor producers,
        IClusterManagerAccessor clusterManagerAccessor)
    {
        _dependencyResolver = dependencyResolver;
        _configuration = configuration;
        _consumerManagerFactory = consumerManagerFactory;
        _clusterManagerAccessor = clusterManagerAccessor;
        this.Consumers = consumers;
        this.Producers = producers;
    }

    public IConsumerAccessor Consumers { get; }

    public IProducerAccessor Producers { get; }

    public async Task StartAsync(CancellationToken stopCancellationToken = default)
    {
        var stopTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stopCancellationToken);

        stopTokenSource.Token.Register(() => this.StopAsync().GetAwaiter().GetResult());

        foreach (var cluster in _configuration.Clusters)
        {
            await this.CreateMissingClusterTopics(cluster);

            foreach (var consumerConfiguration in cluster.Consumers)
            {
                var consumerDependencyScope = _dependencyResolver.CreateScope();

                var consumerManager =
                    _consumerManagerFactory.Create(consumerConfiguration, consumerDependencyScope.Resolver);

                _consumerManagers.Add(consumerManager);
                this.Consumers.Add(
                    new MessageConsumer(
                        consumerManager,
                        consumerDependencyScope.Resolver.Resolve<ILogHandler>()));

                if (consumerConfiguration.InitialState == ConsumerInitialState.Running)
                {
                    await consumerManager.StartAsync().ConfigureAwait(false);
                }
            }

            cluster.OnStartedHandler(_dependencyResolver);
        }
    }

    public Task StopAsync()
    {
        lock (_consumerManagers)
        {
            if (_stopped)
            {
                return Task.CompletedTask;
            }

            _stopped = true;

            foreach (var cluster in _configuration.Clusters)
            {
                cluster.OnStoppingHandler(_dependencyResolver);
            }

            return Task.WhenAll(_consumerManagers.Select(x => x.StopAsync()));
        }
    }

    private async Task CreateMissingClusterTopics(ClusterConfiguration cluster)
    {
        if (cluster.TopicsToCreateIfNotExist is { Count: 0 })
        {
            return;
        }

        await _clusterManagerAccessor[cluster.Name].CreateIfNotExistsAsync(
            cluster.TopicsToCreateIfNotExist);
    }
}
