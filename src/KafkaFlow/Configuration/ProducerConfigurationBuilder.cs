using System;
using System.Collections.Generic;
using Confluent.Kafka;

namespace KafkaFlow.Configuration
{
    internal class ProducerConfigurationBuilder : IProducerConfigurationBuilder
    {
        private readonly string _name;
        private readonly ProducerMiddlewareConfigurationBuilder _middlewareConfigurationBuilder;
        private readonly List<Action<string>> _statisticsHandlers = new();

        private string _topic;
        private ProducerConfig _producerConfig;
        private Acks? _acks;
        private int _statisticsInterval;
        private double? _lingerMs;
        private ProducerCustomFactory _customFactory = (producer, _) => producer;

        public ProducerConfigurationBuilder(IDependencyConfigurator dependencyConfigurator, string name)
        {
            _name = name;
            this.DependencyConfigurator = dependencyConfigurator;
            _middlewareConfigurationBuilder = new ProducerMiddlewareConfigurationBuilder(dependencyConfigurator);
        }

        public IDependencyConfigurator DependencyConfigurator { get; }

        public IProducerConfigurationBuilder AddMiddlewares(Action<IProducerMiddlewareConfigurationBuilder> middlewares)
        {
            middlewares(_middlewareConfigurationBuilder);
            return this;
        }

        public IProducerConfigurationBuilder DefaultTopic(string topic)
        {
            _topic = topic;
            return this;
        }

        public IProducerConfigurationBuilder WithProducerConfig(ProducerConfig config)
        {
            _producerConfig = config;
            return this;
        }

        public IProducerConfigurationBuilder WithCompression(CompressionType compressionType, int? compressionLevel)
        {
            _producerConfig ??= new ProducerConfig();
            _producerConfig.CompressionType = compressionType;
            _producerConfig.CompressionLevel = compressionLevel;
            return this;
        }

        public IProducerConfigurationBuilder WithAcks(Acks acks)
        {
            _acks = acks;
            return this;
        }

        public IProducerConfigurationBuilder WithLingerMs(double lingerMs)
        {
            _lingerMs = lingerMs;
            return this;
        }

        public IProducerConfigurationBuilder WithStatisticsHandler(Action<string> statisticsHandler)
        {
            _statisticsHandlers.Add(statisticsHandler);
            return this;
        }

        public IProducerConfigurationBuilder WithStatisticsIntervalMs(int statisticsIntervalMs)
        {
            _statisticsInterval = statisticsIntervalMs;
            return this;
        }

        public IProducerConfigurationBuilder WithCustomFactory(ProducerCustomFactory customFactory)
        {
            _customFactory = customFactory;
            return this;
        }

        public IProducerConfiguration Build(ClusterConfiguration clusterConfiguration)
        {
            _producerConfig ??= new ProducerConfig();

            _producerConfig.StatisticsIntervalMs = _statisticsInterval;
            _producerConfig.LingerMs = _lingerMs;

            _producerConfig.ReadSecurityInformationFrom(clusterConfiguration);

            var configuration = new ProducerConfiguration(
                clusterConfiguration,
                _name,
                _topic,
                _acks,
                _middlewareConfigurationBuilder.Build(),
                _producerConfig,
                _statisticsHandlers,
                _customFactory);

            return configuration;
        }
    }
}
