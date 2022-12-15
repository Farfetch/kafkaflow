namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Confluent.Kafka;
    using KafkaFlow.Consumers.DistributionStrategies;

    internal sealed class ConsumerConfigurationBuilder : IConsumerConfigurationBuilder
    {
        private readonly List<string> topics = new();
        private readonly List<TopicPartitions> topicsPartitions = new();
        private readonly List<Action<string>> statisticsHandlers = new();

        private readonly List<(Action<IDependencyResolver, IEnumerable<TopicPartitionOffset>>, TimeSpan interval)>
            pendingOffsetsStatisticsHandlers = new();

        private readonly List<Action<IDependencyResolver, IReadOnlyList<TopicPartition>>> partitionAssignedHandlers = new();
        private readonly List<Action<IDependencyResolver, IReadOnlyList<TopicPartitionOffset>>> partitionRevokedHandlers = new();
        private readonly ConsumerMiddlewareConfigurationBuilder middlewareConfigurationBuilder;

        private ConsumerConfig consumerConfig;

        private string name;
        private bool disableManagement;
        private string groupId = string.Empty;
        private AutoOffsetReset? autoOffsetReset;
        private int? maxPollIntervalMs;
        private int workersCount;
        private int bufferSize;
        private TimeSpan workerStopTimeout = TimeSpan.FromSeconds(30);
        private bool autoStoreOffsets = true;
        private bool noStoreOffsets;
        private ConsumerInitialState initialState = ConsumerInitialState.Running;
        private int statisticsInterval;

        private Factory<IDistributionStrategy> distributionStrategyFactory = _ => new BytesSumDistributionStrategy();
        private TimeSpan autoCommitInterval = TimeSpan.FromSeconds(5);

        private ConsumerCustomFactory customFactory = (consumer, _) => consumer;

        public ConsumerConfigurationBuilder(IDependencyConfigurator dependencyConfigurator)
        {
            this.DependencyConfigurator = dependencyConfigurator;
            this.middlewareConfigurationBuilder = new ConsumerMiddlewareConfigurationBuilder(dependencyConfigurator);
        }

        public IDependencyConfigurator DependencyConfigurator { get; }

        public IConsumerConfigurationBuilder Topic(string topicName)
        {
            this.topics.Add(topicName);
            return this;
        }

        public IConsumerConfigurationBuilder ManualAssignPartitions(string topicName, IEnumerable<int> partitions)
        {
            this.topicsPartitions.Add(new TopicPartitions(topicName, partitions));
            return this;
        }

        public IConsumerConfigurationBuilder WithConsumerConfig(ConsumerConfig config)
        {
            this.consumerConfig = config;
            return this;
        }

        public IConsumerConfigurationBuilder Topics(IEnumerable<string> topicNames)
        {
            this.topics.AddRange(topicNames);
            return this;
        }

        public IConsumerConfigurationBuilder Topics(params string[] topicNames) => this.Topics(topicNames.AsEnumerable());

        public IConsumerConfigurationBuilder WithName(string name)
        {
            this.name = name;
            return this;
        }

        public IConsumerConfigurationBuilder DisableManagement()
        {
            this.disableManagement = true;
            return this;
        }

        public IConsumerConfigurationBuilder WithGroupId(string groupId)
        {
            this.groupId = groupId;
            return this;
        }

        public IConsumerConfigurationBuilder WithAutoOffsetReset(KafkaFlow.AutoOffsetReset autoOffsetReset)
        {
            this.autoOffsetReset = autoOffsetReset switch
            {
                KafkaFlow.AutoOffsetReset.Earliest => AutoOffsetReset.Earliest,
                KafkaFlow.AutoOffsetReset.Latest => AutoOffsetReset.Latest,
                _ => throw new InvalidEnumArgumentException(
                    nameof(autoOffsetReset),
                    (int)autoOffsetReset,
                    typeof(KafkaFlow.AutoOffsetReset))
            };

            return this;
        }

        public IConsumerConfigurationBuilder WithAutoCommitIntervalMs(int autoCommitIntervalMs)
        {
            this.autoCommitInterval = TimeSpan.FromMilliseconds(autoCommitIntervalMs);
            return this;
        }

        public IConsumerConfigurationBuilder WithMaxPollIntervalMs(int maxPollIntervalMs)
        {
            this.maxPollIntervalMs = maxPollIntervalMs;
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

        public IConsumerConfigurationBuilder WithWorkerStopTimeout(int seconds)
        {
            this.workerStopTimeout = TimeSpan.FromSeconds(seconds);
            return this;
        }

        public IConsumerConfigurationBuilder WithWorkerStopTimeout(TimeSpan timeout)
        {
            this.workerStopTimeout = timeout;
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

        public IConsumerConfigurationBuilder WithoutStoringOffsets()
        {
            this.noStoreOffsets = true;
            return this;
        }

        public IConsumerConfigurationBuilder WithInitialState(ConsumerInitialState state)
        {
            this.initialState = state;
            return this;
        }

        public IConsumerConfigurationBuilder AddMiddlewares(Action<IConsumerMiddlewareConfigurationBuilder> middlewares)
        {
            middlewares(this.middlewareConfigurationBuilder);
            return this;
        }

        public IConsumerConfigurationBuilder WithPartitionsAssignedHandler(
            Action<IDependencyResolver, IReadOnlyList<TopicPartition>> partitionsAssignedHandler)
        {
            this.partitionAssignedHandlers.Add(partitionsAssignedHandler);
            return this;
        }

        public IConsumerConfigurationBuilder WithPartitionsRevokedHandler(
            Action<IDependencyResolver, IReadOnlyList<TopicPartitionOffset>> partitionsRevokedHandler)
        {
            this.partitionRevokedHandlers.Add(partitionsRevokedHandler);
            return this;
        }

        public IConsumerConfigurationBuilder WithStatisticsHandler(Action<string> statisticsHandler)
        {
            this.statisticsHandlers.Add(statisticsHandler);
            return this;
        }

        public IConsumerConfigurationBuilder WithStatisticsIntervalMs(int statisticsIntervalMs)
        {
            this.statisticsInterval = statisticsIntervalMs;
            return this;
        }

        public IConsumerConfigurationBuilder WithPendingOffsetsStatisticsHandler(
            Action<IDependencyResolver, IEnumerable<TopicPartitionOffset>> pendingOffsetsHandler,
            TimeSpan interval)
        {
            this.pendingOffsetsStatisticsHandlers.Add((pendingOffsetsHandler, interval));
            return this;
        }

        public IConsumerConfigurationBuilder WithCustomFactory(ConsumerCustomFactory customFactory)
        {
            this.customFactory = customFactory;
            return this;
        }

        public IConsumerConfiguration Build(ClusterConfiguration clusterConfiguration)
        {
            var middlewareConfiguration = this.middlewareConfigurationBuilder.Build();

            this.consumerConfig ??= new ConsumerConfig();
            this.consumerConfig.BootstrapServers ??= string.Join(",", clusterConfiguration.Brokers);
            this.consumerConfig.GroupId ??= this.groupId;
            this.consumerConfig.AutoOffsetReset ??= this.autoOffsetReset;
            this.consumerConfig.MaxPollIntervalMs ??= this.maxPollIntervalMs;
            this.consumerConfig.StatisticsIntervalMs ??= this.statisticsInterval;

            this.consumerConfig.EnableAutoOffsetStore = false;
            this.consumerConfig.EnableAutoCommit = false;

            this.consumerConfig.ReadSecurityInformationFrom(clusterConfiguration);

            return new ConsumerConfiguration(
                this.consumerConfig,
                this.topics,
                this.topicsPartitions,
                this.name,
                clusterConfiguration,
                this.disableManagement,
                this.workersCount,
                this.bufferSize,
                this.workerStopTimeout,
                this.distributionStrategyFactory,
                middlewareConfiguration,
                this.autoStoreOffsets,
                this.noStoreOffsets,
                this.initialState,
                this.autoCommitInterval,
                this.statisticsHandlers,
                this.partitionAssignedHandlers,
                this.partitionRevokedHandlers,
                this.pendingOffsetsStatisticsHandlers,
                this.customFactory);
        }
    }
}
