using System;
using System.Collections.Generic;
using System.Linq;
using KafkaFlow.Producers;

namespace KafkaFlow.Configuration
{
    internal class ClusterConfigurationBuilder : IClusterConfigurationBuilder
    {
        private readonly List<ProducerConfigurationBuilder> _producers = new();
        private readonly List<ConsumerConfigurationBuilder> _consumers = new();
        private readonly List<TopicConfiguration> _topicsToCreateIfNotExist = new();
        private Action<IDependencyResolver> _onStartedHandler = _ => { };
        private Action<IDependencyResolver> _onStoppingHandler = _ => { };
        private IEnumerable<string> _brokers;
        private string _name;
        private Func<SecurityInformation> _securityInformationHandler;

        public ClusterConfigurationBuilder(IDependencyConfigurator dependencyConfigurator)
        {
            this.DependencyConfigurator = dependencyConfigurator;
        }

        public IDependencyConfigurator DependencyConfigurator { get; }

        public ClusterConfiguration Build(KafkaConfiguration kafkaConfiguration)
        {
            var configuration = new ClusterConfiguration(
                kafkaConfiguration,
                _name,
                _brokers.ToList(),
                _securityInformationHandler,
                _onStartedHandler,
                _onStoppingHandler,
                _topicsToCreateIfNotExist);

            configuration.AddProducers(_producers.Select(x => x.Build(configuration)));
            configuration.AddConsumers(_consumers.Select(x => x.Build(configuration)));

            return configuration;
        }

        public IClusterConfigurationBuilder WithBrokers(IEnumerable<string> brokers)
        {
            _brokers = brokers;
            return this;
        }

        public IClusterConfigurationBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public IClusterConfigurationBuilder WithSecurityInformation(Action<SecurityInformation> handler)
        {
            // Uses a handler to avoid in-memory stored passwords for long periods
            _securityInformationHandler = () =>
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

            _producers.Add(builder);

            return this;
        }

        public IClusterConfigurationBuilder AddConsumer(Action<IConsumerConfigurationBuilder> consumer)
        {
            var builder = new ConsumerConfigurationBuilder(this.DependencyConfigurator);

            consumer(builder);

            _consumers.Add(builder);

            return this;
        }

        public IClusterConfigurationBuilder OnStopping(Action<IDependencyResolver> handler)
        {
            _onStoppingHandler = handler;
            return this;
        }

        public IClusterConfigurationBuilder OnStarted(Action<IDependencyResolver> handler)
        {
            _onStartedHandler = handler;
            return this;
        }

        public IClusterConfigurationBuilder CreateTopicIfNotExists(
            string topicName,
            int numberOfPartitions,
            short replicationFactor)
        {
            _topicsToCreateIfNotExist.Add(new TopicConfiguration(topicName, numberOfPartitions, replicationFactor));
            return this;
        }
    }
}
