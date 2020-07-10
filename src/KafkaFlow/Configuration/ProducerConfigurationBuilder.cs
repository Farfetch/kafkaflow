namespace KafkaFlow.Configuration
{
    using System;
    using Confluent.Kafka;
    using KafkaFlow.Producers;
    using Acks = KafkaFlow.Acks;

    internal class ProducerConfigurationBuilder : IProducerConfigurationBuilder
    {
        private readonly Type producerType;
        private readonly ProducerMiddlewareConfigurationBuilder middlewareConfigurationBuilder;

        private string topic;
        private ProducerConfig baseProducerConfig;
        private Acks? acks;
        private string name;

        public ProducerConfigurationBuilder(IDependencyConfigurator dependencyConfigurator, Type type)
        {
            this.DependencyConfigurator = dependencyConfigurator;
            this.producerType = type;
            this.middlewareConfigurationBuilder = new ProducerMiddlewareConfigurationBuilder(dependencyConfigurator);
        }

        public IDependencyConfigurator DependencyConfigurator { get; }

        public IProducerConfigurationBuilder AddMiddlewares(Action<IProducerMiddlewareConfigurationBuilder> middlewares)
        {
            middlewares(this.middlewareConfigurationBuilder);
            return this;
        }

        public IProducerConfigurationBuilder DefaultTopic(string topic)
        {
            this.topic = topic;
            return this;
        }

        public IProducerConfigurationBuilder WithProducerConfig(ProducerConfig config)
        {
            this.baseProducerConfig = config;
            return this;
        }

        public IProducerConfigurationBuilder WithAcks(Acks acks)
        {
            this.acks = acks;
            return this;
        }

        public IProducerConfigurationBuilder WithName(string name)
        {
            this.name = name;
            return this;
        }

        public ProducerConfiguration Build(ClusterConfiguration clusterConfiguration)
        {
            var configuration = new ProducerConfiguration(
                clusterConfiguration,
                this.topic,
                this.acks,
                this.middlewareConfigurationBuilder.Build(),
                this.baseProducerConfig ?? new ProducerConfig());

            this.DependencyConfigurator.AddSingleton(
                typeof(IMessageProducer<>).MakeGenericType(this.producerType),
                resolver => Activator.CreateInstance(
                    typeof(MessageProducer<>).MakeGenericType(this.producerType),
                    resolver,
                    this.name,
                    configuration));

            return configuration;
        }
    }
}
