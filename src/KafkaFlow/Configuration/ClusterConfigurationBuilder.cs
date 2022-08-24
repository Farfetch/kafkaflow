namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using KafkaFlow.Producers;

    internal class ClusterConfigurationBuilder : IClusterConfigurationBuilder
    {
        private readonly List<ProducerConfigurationBuilder> producers = new();
        private readonly List<ConsumerConfigurationBuilder> consumers = new();
        private readonly List<TopicConfiguration> topicsToCreateIfNotExist = new();
        private Action<IDependencyResolver> onStartedHandler = _ => { };
        private Action<IDependencyResolver> onStoppingHandler = _ => { };
        private IEnumerable<string> brokers;
        private string name;
        private Func<SecurityInformation> securityInformationHandler;

        public ClusterConfigurationBuilder(IDependencyConfigurator dependencyConfigurator)
        {
            this.DependencyConfigurator = dependencyConfigurator;
        }

        public IDependencyConfigurator DependencyConfigurator { get; }

        public ClusterConfiguration Build(KafkaConfiguration kafkaConfiguration)
        {
            var configuration = new ClusterConfiguration(
                kafkaConfiguration,
                this.name,
                this.brokers.ToList(),
                this.securityInformationHandler,
                this.onStartedHandler,
                this.onStoppingHandler);

            configuration.AddProducers(this.producers.Select(x => x.Build(configuration)));
            configuration.AddConsumers(this.consumers.Select(x => x.Build(configuration)));
            configuration.AddTopicsToCreateIfNotExists(this.topicsToCreateIfNotExist);

            return configuration;
        }

        public IClusterConfigurationBuilder WithBrokers(IEnumerable<string> brokers)
        {
            this.brokers = brokers;
            return this;
        }

        public IClusterConfigurationBuilder WithName(string name)
        {
            this.name = name;
            return this;
        }

        public IClusterConfigurationBuilder WithSecurityInformation(Action<SecurityInformation> handler)
        {
            // Uses a handler to avoid in-memory stored passwords for long periods
            this.securityInformationHandler = () =>
            {
                var config = new SecurityInformation();
                handler(config);
                return config;
            };

            return this;
        }

        public IClusterConfigurationBuilder AddProducer<TProducer>(Action<IProducerConfigurationBuilder> producer)
        {
            this.DependencyConfigurator.AddSingleton<IMessageProducer<TProducer>>(
                resolver => new MessageProducerWrapper<TProducer>(
                    resolver.Resolve<IProducerAccessor>().GetProducer<TProducer>()));

            return this.AddProducer(typeof(TProducer).FullName, producer);
        }

        public IClusterConfigurationBuilder AddProducer(string name, Action<IProducerConfigurationBuilder> producer)
        {
            var builder = new ProducerConfigurationBuilder(this.DependencyConfigurator, name);

            producer(builder);

            this.producers.Add(builder);

            return this;
        }

        public IClusterConfigurationBuilder AddConsumer(Action<IConsumerConfigurationBuilder> consumer)
        {
            var builder = new ConsumerConfigurationBuilder(this.DependencyConfigurator);

            consumer(builder);

            this.consumers.Add(builder);

            return this;
        }

        public IClusterConfigurationBuilder OnStopping(Action<IDependencyResolver> handler)
        {
            this.onStoppingHandler = handler;
            return this;
        }

        public IClusterConfigurationBuilder OnStarted(Action<IDependencyResolver> handler)
        {
            this.onStartedHandler = handler;
            return this;
        }

        public IClusterConfigurationBuilder CreateTopicIfNotExists(
            string topicName,
            int numberOfPartitions,
            short replicationFactor)
        {
            this.topicsToCreateIfNotExist.Add(new TopicConfiguration(topicName, numberOfPartitions, replicationFactor));
            return this;
        }
    }
}
