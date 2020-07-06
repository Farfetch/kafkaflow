namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Confluent.Kafka;
    using KafkaFlow.Consumers.DistributionStrategies;

    internal sealed class ConsumerConfigurationBuilder
        : IConsumerConfigurationBuilder
    {
        private readonly List<string> topics = new List<string>();
        private readonly ConsumerMiddlewareConfigurationBuilder middlewareConfigurationBuilder;

        private ConsumerConfig consumerConfig;

        private string name;
        private int workersCount;
        private int bufferSize;
        private bool autoStoreOffsets = true;

        private Factory<IDistributionStrategy> distributionStrategyFactory = resolver => new BytesSumDistributionStrategy();
        
        public IDependencyConfigurator DependencyConfigurator { get; }

        public ConsumerConfigurationBuilder(IDependencyConfigurator dependencyConfigurator)
        {
            this.DependencyConfigurator = dependencyConfigurator;
            this.middlewareConfigurationBuilder = new ConsumerMiddlewareConfigurationBuilder(dependencyConfigurator);
            this.consumerConfig = new ConsumerConfig();
        }

        public IConsumerConfigurationBuilder Topic(string topic)
        {
            this.topics.Add(topic);
            return this;
        }

        public IConsumerConfigurationBuilder WithConsumerConfig(ConsumerConfig config)
        {
            this.consumerConfig = config;
            return this;
        }

        public IConsumerConfigurationBuilder Topics(IEnumerable<string> topics)
        {
            this.topics.AddRange(topics);
            return this;
        }

        public IConsumerConfigurationBuilder Topics(params string[] topics) => this.Topics(topics.AsEnumerable());

        public IConsumerConfigurationBuilder WithName(string name)
        {
            this.name = name;
            return this;
        }

        public IConsumerConfigurationBuilder WithGroupId(string groupId)
        {
            this.consumerConfig.GroupId = groupId;
            return this;
        }

        public IConsumerConfigurationBuilder WithAutoOffsetReset(KafkaFlow.AutoOffsetReset autoOffsetReset)
        {
            this.consumerConfig.AutoOffsetReset = autoOffsetReset switch
            {
                KafkaFlow.AutoOffsetReset.Earliest => AutoOffsetReset.Earliest,
                KafkaFlow.AutoOffsetReset.Latest => AutoOffsetReset.Latest,
                _ => throw new InvalidEnumArgumentException(
                    nameof(autoOffsetReset),
                    (int) autoOffsetReset,
                    typeof(KafkaFlow.AutoOffsetReset))
            };

            return this;
        }

        public IConsumerConfigurationBuilder WithAutoCommitIntervalMs(int autoCommitIntervalMs)
        {
            this.consumerConfig.AutoCommitIntervalMs = autoCommitIntervalMs;
            return this;
        }

        public IConsumerConfigurationBuilder WithMaxPollIntervalMs(int maxPollIntervalMs)
        {
            this.consumerConfig.MaxPollIntervalMs = maxPollIntervalMs;
            return this;
        }

        public IConsumerConfigurationBuilder WithWorkersCount(int workersCount)
        {
            this.workersCount = workersCount;
            return this;
        }

        public IConsumerConfigurationBuilder WithBufferSize(int size)
        {
            this.bufferSize = size;
            return this;
        }

        public IConsumerConfigurationBuilder WithWorkDistributionStrategy<T>(Factory<T> factory)
            where T : class, IDistributionStrategy
        {
            this.distributionStrategyFactory = factory;

            return this;
        }

        public IConsumerConfigurationBuilder WithWorkDistributionStrategy<T>()
            where T : class, IDistributionStrategy
        {
            this.DependencyConfigurator.AddTransient<T>();
            this.distributionStrategyFactory = resolver => resolver.Resolve<T>();

            return this;
        }

        public IConsumerConfigurationBuilder WithAutoStoreOffsets()
        {
            this.autoStoreOffsets = true;
            return this;
        }

        public IConsumerConfigurationBuilder WithManualStoreOffsets()
        {
            this.autoStoreOffsets = false;
            return this;
        }

        public IConsumerConfigurationBuilder AddMiddlewares(Action<IConsumerMiddlewareConfigurationBuilder> middlewares)
        {
            middlewares(this.middlewareConfigurationBuilder);
            return this;
        }

        public ConsumerConfiguration Build(ClusterConfiguration clusterConfiguration)
        {
            var middlewareConfiguration = this.middlewareConfigurationBuilder.Build();

            this.consumerConfig.BootstrapServers = string.Join(",", clusterConfiguration.Brokers);
            this.consumerConfig.EnableAutoOffsetStore = false;
            this.consumerConfig.EnableAutoCommit = true;

            return new ConsumerConfiguration(
                this.consumerConfig,
                this.topics,
                this.name,
                this.workersCount,
                this.bufferSize,
                this.distributionStrategyFactory,
                middlewareConfiguration
            )
            {
                AutoStoreOffsets = this.autoStoreOffsets
            };
        }
    }
}
