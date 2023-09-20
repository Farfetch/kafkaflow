namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using KafkaFlow.Consumers.DistributionStrategies;

    internal sealed class ConsumerConfigurationBuilder : IConsumerConfigurationBuilder
    {
        private readonly List<string> topics = new();
        private readonly List<TopicPartitions> topicsPartitions = new();
        private readonly List<Action<string>> statisticsHandlers = new();

        private readonly List<PendingOffsetsStatisticsHandler> pendingOffsetsStatisticsHandlers = new();

        private readonly List<Action<IDependencyResolver, List<TopicPartition>>> partitionAssignedHandlers = new();
        private readonly List<Action<IDependencyResolver, List<TopicPartitionOffset>>> partitionRevokedHandlers = new();
        private readonly ConsumerMiddlewareConfigurationBuilder middlewareConfigurationBuilder;

        private ConsumerConfig consumerConfig;

        private string name;
        private bool disableManagement;
        private string groupId = string.Empty;
        private AutoOffsetReset? autoOffsetReset;
        private int? maxPollIntervalMs;
        private Func<WorkersCountContext, IDependencyResolver, Task<int>> workersCountCalculator;
        private TimeSpan workersCountEvaluationInterval = TimeSpan.FromMinutes(5);
        private int bufferSize;
        private TimeSpan workerStopTimeout = TimeSpan.FromSeconds(30);
        private bool autoMessageCompletion = true;
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

        public IConsumerConfigurationBuilder WithWorkersCount(
            Func<WorkersCountContext, IDependencyResolver, Task<int>> calculator,
            TimeSpan evaluationInterval)
        {
            this.workersCountCalculator = calculator;
            this.workersCountEvaluationInterval = evaluationInterval;
            return this;
        }

        public IConsumerConfigurationBuilder WithWorkersCount(Func<WorkersCountContext, IDependencyResolver, Task<int>> calculator)
        {
            return this.WithWorkersCount(calculator, TimeSpan.FromMinutes(5));
        }

        public IConsumerConfigurationBuilder WithWorkersCount(int workersCount)
        {
            return this.WithWorkersCount((_, _) => Task.FromResult(workersCount));
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

        public IConsumerConfigurationBuilder WithManualMessageCompletion()
        {
            this.autoMessageCompletion = false;
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
            Action<IDependencyResolver, List<TopicPartition>> partitionsAssignedHandler)
        {
            this.partitionAssignedHandlers.Add(partitionsAssignedHandler);
            return this;
        }

        public IConsumerConfigurationBuilder WithPartitionsRevokedHandler(
            Action<IDependencyResolver, List<TopicPartitionOffset>> partitionsRevokedHandler)
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
            this.pendingOffsetsStatisticsHandlers.Add(new(pendingOffsetsHandler, interval));
            return this;
        }

        public IConsumerConfigurationBuilder WithCustomFactory(ConsumerCustomFactory customFactory)
        {
            this.customFactory = customFactory;
            return this;
        }

        public void AddInstrumentation<T>(IConsumerInstrumentationMiddleware consumerInstrumentationMiddleware)
            where T : class, IMessageMiddleware
        {
            var type = typeof(IConsumerInstrumentationMiddleware);            
        }

        public IConsumerConfiguration Build(ClusterConfiguration clusterConfiguration)
        {
            var middlewareConfiguration = this.middlewareConfigurationBuilder.Build();

            this.consumerConfig ??= new ConsumerConfig();

            var consumerConfigCopy = new ConsumerConfig(this.consumerConfig.ToDictionary(x => x.Key, x => x.Value));

            consumerConfigCopy.BootstrapServers = this.consumerConfig.BootstrapServers ?? string.Join(",", clusterConfiguration.Brokers);
            consumerConfigCopy.GroupId = this.consumerConfig.GroupId ?? this.groupId;
            consumerConfigCopy.AutoOffsetReset = this.consumerConfig.AutoOffsetReset ?? this.autoOffsetReset;
            consumerConfigCopy.MaxPollIntervalMs = this.consumerConfig.MaxPollIntervalMs ?? this.maxPollIntervalMs;
            consumerConfigCopy.StatisticsIntervalMs = this.consumerConfig.StatisticsIntervalMs ?? this.statisticsInterval;

            consumerConfigCopy.EnableAutoOffsetStore = false;
            consumerConfigCopy.EnableAutoCommit = false;

            consumerConfigCopy.ReadSecurityInformationFrom(clusterConfiguration);

            return new ConsumerConfiguration(
                consumerConfigCopy,
                this.topics,
                this.topicsPartitions,
                this.name,
                clusterConfiguration,
                this.disableManagement,
                this.workersCountCalculator,
                this.workersCountEvaluationInterval,
                this.bufferSize,
                this.workerStopTimeout,
                this.distributionStrategyFactory,
                middlewareConfiguration,
                this.autoMessageCompletion,
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
