using System;
using System.Collections.Generic;
using System.Linq;
using KafkaFlow.Clusters;
using KafkaFlow.Consumers;
using KafkaFlow.Producers;

namespace KafkaFlow.Configuration;

internal class KafkaConfigurationBuilder : IKafkaConfigurationBuilder
{
    private readonly IDependencyConfigurator _dependencyConfigurator;
    private readonly List<ClusterConfigurationBuilder> _clusters = new();
    private readonly IList<Action<IGlobalEvents>> _globalEventsConfigurators = new List<Action<IGlobalEvents>>();
    private Type _logHandlerType = typeof(NullLogHandler);

    public KafkaConfigurationBuilder(IDependencyConfigurator dependencyConfigurator)
    {
        _dependencyConfigurator = dependencyConfigurator;
    }

    public KafkaConfiguration Build()
    {
        var configuration = new KafkaConfiguration();

        configuration.AddClusters(_clusters.Select(x => x.Build(configuration)));

        _dependencyConfigurator.AddSingleton<IProducerAccessor>(
            resolver => new ProducerAccessor(
                configuration.Clusters
                    .SelectMany(x => x.Producers)
                    .Select(
                        producer => new MessageProducer(
                            resolver,
                            producer))));

        foreach (var cluster in configuration.Clusters)
        {
            _dependencyConfigurator.AddSingleton<IClusterManager>(
                resolver =>
                    new ClusterManager(resolver.Resolve<IAdminClientBuilderFactory>(), resolver.Resolve<ILogHandler>(), cluster));
        }

        _dependencyConfigurator
            .AddTransient(typeof(ILogHandler), _logHandlerType)
            .AddSingleton<IDateTimeProvider, DateTimeProvider>()
            .AddSingleton<IAdminClientBuilderFactory, AdminClientBuilderFactory>()
            .AddSingleton<IProducerBuilderFactory, ProducerBuilderFactory>()
            .AddSingleton<IConsumerBuilderFactory, ConsumerBuilderFactory>()
            .AddSingleton<IConsumerAccessor>(new ConsumerAccessor())
            .AddSingleton<IConsumerManagerFactory>(new ConsumerManagerFactory())
            .AddSingleton<IClusterManagerAccessor, ClusterManagerAccessor>()
            .AddScoped<ConsumerMiddlewareContext>()
            .AddScoped<IConsumerMiddlewareContext>(r => r.Resolve<ConsumerMiddlewareContext>())
            .AddSingleton(
                r =>
                {
                    var logHandler = r.Resolve<ILogHandler>();

                    var globalEvents = new GlobalEvents(logHandler);

                    foreach (var del in _globalEventsConfigurators)
                    {
                        del.Invoke(globalEvents);
                    }

                    return globalEvents;
                });

        return configuration;
    }

    public IKafkaConfigurationBuilder AddCluster(Action<IClusterConfigurationBuilder> cluster)
    {
        var builder = new ClusterConfigurationBuilder(_dependencyConfigurator);

        cluster(builder);

        _clusters.Add(builder);

        return this;
    }

    public IKafkaConfigurationBuilder UseLogHandler<TLogHandler>()
        where TLogHandler : ILogHandler
    {
        _logHandlerType = typeof(TLogHandler);
        return this;
    }

    public IKafkaConfigurationBuilder SubscribeGlobalEvents(Action<IGlobalEvents> observers)
    {
        _globalEventsConfigurators.Add(observers);

        return this;
    }
}
