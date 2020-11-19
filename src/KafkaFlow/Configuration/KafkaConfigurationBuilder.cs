namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using KafkaFlow.Consumers;
    using KafkaFlow.Producers;

    internal class KafkaConfigurationBuilder : IKafkaConfigurationBuilder
    {
        private readonly List<ClusterConfigurationBuilder> clusters = new List<ClusterConfigurationBuilder>();
        private Type logHandler = typeof(NullLogHandler);

        public KafkaConfigurationBuilder(IDependencyConfigurator dependencyConfigurator)
        {
            this.DependencyConfigurator = dependencyConfigurator;
        }

        public IDependencyConfigurator DependencyConfigurator { get; }

        public KafkaConfiguration Build()
        {
            var configuration = new KafkaConfiguration();

            configuration.AddClusters(this.clusters.Select(x => x.Build(configuration)));

            this.DependencyConfigurator.AddSingleton<IProducerAccessor>(
                resolver => new ProducerAccessor(
                    configuration.Clusters
                        .SelectMany(x => x.Producers)
                        .Select(
                            producer => new MessageProducer(
                                resolver,
                                producer))));

            var consumerManager = new ConsumerManager();

            this.DependencyConfigurator
                .AddTransient(typeof(ILogHandler), this.logHandler)
                .AddSingleton<IConsumerAccessor>(consumerManager)
                .AddSingleton<IConsumerManager>(consumerManager);

            return configuration;
        }

        public IKafkaConfigurationBuilder AddCluster(Action<IClusterConfigurationBuilder> cluster)
        {
            var builder = new ClusterConfigurationBuilder(this.DependencyConfigurator);

            cluster(builder);

            this.clusters.Add(builder);

            return this;
        }

        public IKafkaConfigurationBuilder UseLogHandler<TLogHandler>() where TLogHandler : ILogHandler
        {
            this.logHandler = typeof(TLogHandler);
            return this;
        }
    }
}
