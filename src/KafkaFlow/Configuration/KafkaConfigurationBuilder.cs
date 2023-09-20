namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using KafkaFlow.Clusters;
    using KafkaFlow.Consumers;
    using KafkaFlow.Events;
    using KafkaFlow.Producers;

    internal class KafkaConfigurationBuilder : IKafkaConfigurationBuilder
    {
        private readonly IDependencyConfigurator dependencyConfigurator;

        private readonly List<ClusterConfigurationBuilder> clusters = new();
        private Type logHandlerType = typeof(NullLogHandler);
        private readonly EventsManager eventsManager;

        public KafkaConfigurationBuilder(IDependencyConfigurator dependencyConfigurator)
        {
            this.dependencyConfigurator = dependencyConfigurator;
            this.eventsManager = new EventsManager();
        }

        public KafkaConfiguration Build()
        {
            var configuration = new KafkaConfiguration();

            configuration.AddClusters(this.clusters.Select(x => x.Build(configuration)));

            this.dependencyConfigurator.AddSingleton<IProducerAccessor>(
                resolver => new ProducerAccessor(
                    configuration.Clusters
                        .SelectMany(x => x.Producers)
                        .Select(
                            producer => new MessageProducer(
                                resolver,
                                producer))));

            foreach (var cluster in configuration.Clusters)
            {
                this.dependencyConfigurator.AddSingleton<IClusterManager>(resolver =>
                     new ClusterManager(resolver.Resolve<ILogHandler>(), cluster));
            }

            this.dependencyConfigurator
                .AddTransient(typeof(ILogHandler), this.logHandlerType)
                .AddSingleton<IDateTimeProvider, DateTimeProvider>()
                .AddSingleton<IConsumerAccessor>(new ConsumerAccessor())
                .AddSingleton<IConsumerManagerFactory>(new ConsumerManagerFactory())
                .AddSingleton<IClusterManagerAccessor, ClusterManagerAccessor>()
                .AddSingleton<IEventsListener>(this.eventsManager)
                .AddSingleton<IEventsNotifier>(this.eventsManager);

            return configuration;
        }

        public IKafkaConfigurationBuilder AddCluster(Action<IClusterConfigurationBuilder> cluster)
        {
            var builder = new ClusterConfigurationBuilder(this.dependencyConfigurator);

            cluster(builder);

            this.clusters.Add(builder);

            return this;
        }

        public IKafkaConfigurationBuilder UseLogHandler<TLogHandler>()
            where TLogHandler : ILogHandler
        {
            this.logHandlerType = typeof(TLogHandler);
            return this;
        }

        public IKafkaConfigurationBuilder SubscribeEvents(Action<IEventsListener> eventsListener)
        {
            eventsListener?.Invoke(this.eventsManager);
            return this;
        }
    }
}
