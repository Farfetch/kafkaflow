using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using KafkaFlow.Consumers.DistributionStrategies;
using KafkaFlow.Extensions;

namespace KafkaFlow.Configuration;

internal sealed class ConsumerConfigurationBuilder : IConsumerConfigurationBuilder
{
    private readonly List<string> _topics = new();
    private readonly List<TopicPartitions> _topicsPartitions = new();
    private readonly List<Action<string>> _statisticsHandlers = new();

    private readonly List<PendingOffsetsStatisticsHandler> _pendingOffsetsStatisticsHandlers = new();

    private readonly List<Action<IDependencyResolver, List<Confluent.Kafka.TopicPartition>>> _partitionAssignedHandlers = new();
    private readonly List<Action<IDependencyResolver, List<Confluent.Kafka.TopicPartitionOffset>>> _partitionRevokedHandlers = new();
    private readonly ConsumerMiddlewareConfigurationBuilder _middlewareConfigurationBuilder;

    private Confluent.Kafka.ConsumerConfig _consumerConfig;

    private string _name;
    private bool _disableManagement;
    private string _groupId = string.Empty;
    private Confluent.Kafka.AutoOffsetReset? _autoOffsetReset;
    private int? _maxPollIntervalMs;
    private Func<WorkersCountContext, IDependencyResolver, Task<int>> _workersCountCalculator;
    private TimeSpan _workersCountEvaluationInterval = TimeSpan.FromMinutes(5);
    private int _bufferSize;
    private TimeSpan _workerStopTimeout = TimeSpan.FromSeconds(30);
    private bool _autoMessageCompletion = true;
    private bool _noStoreOffsets;
    private ConsumerInitialState _initialState = ConsumerInitialState.Running;
    private int _statisticsInterval;

    private Factory<IWorkerDistributionStrategy> _distributionStrategyFactory = _ => new BytesSumDistributionStrategy();
    private TimeSpan _autoCommitInterval = TimeSpan.FromSeconds(5);

    private ConsumerCustomFactory _customFactory = (consumer, _) => consumer;

    public ConsumerConfigurationBuilder(IDependencyConfigurator dependencyConfigurator)
    {
        this.DependencyConfigurator = dependencyConfigurator;
        _middlewareConfigurationBuilder = new ConsumerMiddlewareConfigurationBuilder(dependencyConfigurator);
    }

    public IDependencyConfigurator DependencyConfigurator { get; }

    public IConsumerConfigurationBuilder Topic(string topicName)
    {
        _topics.Add(topicName);
        return this;
    }

    public IConsumerConfigurationBuilder ManualAssignPartitions(string topicName, IEnumerable<int> partitions)
    {
        _topicsPartitions.Add(new TopicPartitions(topicName, partitions));
        return this;
    }

    public IConsumerConfigurationBuilder WithConsumerConfig(Confluent.Kafka.ConsumerConfig config)
    {
        _consumerConfig = config;
        return this;
    }

    public IConsumerConfigurationBuilder Topics(IEnumerable<string> topicNames)
    {
        _topics.AddRange(topicNames);
        return this;
    }

    public IConsumerConfigurationBuilder Topics(params string[] topicNames) => this.Topics(topicNames.AsEnumerable());

    public IConsumerConfigurationBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public IConsumerConfigurationBuilder DisableManagement()
    {
        _disableManagement = true;
        return this;
    }

    public IConsumerConfigurationBuilder WithGroupId(string groupId)
    {
        _groupId = groupId;
        return this;
    }

    public IConsumerConfigurationBuilder WithAutoOffsetReset(KafkaFlow.AutoOffsetReset autoOffsetReset)
    {
        _autoOffsetReset = autoOffsetReset switch
        {
            KafkaFlow.AutoOffsetReset.Earliest => Confluent.Kafka.AutoOffsetReset.Earliest,
            KafkaFlow.AutoOffsetReset.Latest => Confluent.Kafka.AutoOffsetReset.Latest,
            _ => throw new InvalidEnumArgumentException(
                nameof(autoOffsetReset),
                (int)autoOffsetReset,
                typeof(KafkaFlow.AutoOffsetReset))
        };

        return this;
    }

    public IConsumerConfigurationBuilder WithAutoCommitIntervalMs(int autoCommitIntervalMs)
    {
        _autoCommitInterval = TimeSpan.FromMilliseconds(autoCommitIntervalMs);
        return this;
    }

    public IConsumerConfigurationBuilder WithMaxPollIntervalMs(int maxPollIntervalMs)
    {
        _maxPollIntervalMs = maxPollIntervalMs;
        return this;
    }

    public IConsumerConfigurationBuilder WithWorkersCount(
        Func<WorkersCountContext, IDependencyResolver, Task<int>> calculator,
        TimeSpan evaluationInterval)
    {
        _workersCountCalculator = calculator;
        _workersCountEvaluationInterval = evaluationInterval;
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
        _bufferSize = size;
        return this;
    }

    public IConsumerConfigurationBuilder WithWorkerStopTimeout(int seconds)
    {
        _workerStopTimeout = TimeSpan.FromSeconds(seconds);
        return this;
    }

    public IConsumerConfigurationBuilder WithWorkerStopTimeout(TimeSpan timeout)
    {
        _workerStopTimeout = timeout;
        return this;
    }

    public IConsumerConfigurationBuilder WithWorkerDistributionStrategy<T>(Factory<T> factory)
        where T : class, IWorkerDistributionStrategy
    {
        _distributionStrategyFactory = factory;
        return this;
    }

    public IConsumerConfigurationBuilder WithWorkerDistributionStrategy<T>()
        where T : class, IWorkerDistributionStrategy
    {
        this.DependencyConfigurator.AddTransient<T>();
        _distributionStrategyFactory = resolver => resolver.Resolve<T>();

        return this;
    }

    public IConsumerConfigurationBuilder WithManualMessageCompletion()
    {
        _autoMessageCompletion = false;
        return this;
    }

    public IConsumerConfigurationBuilder WithoutStoringOffsets()
    {
        _noStoreOffsets = true;
        return this;
    }

    public IConsumerConfigurationBuilder WithInitialState(ConsumerInitialState state)
    {
        _initialState = state;
        return this;
    }

    public IConsumerConfigurationBuilder AddMiddlewares(Action<IConsumerMiddlewareConfigurationBuilder> middlewares)
    {
        middlewares(_middlewareConfigurationBuilder);
        return this;
    }

    public IConsumerConfigurationBuilder WithPartitionsAssignedHandler(
        Action<IDependencyResolver, List<Confluent.Kafka.TopicPartition>> partitionsAssignedHandler)
    {
        _partitionAssignedHandlers.Add(partitionsAssignedHandler);
        return this;
    }

    public IConsumerConfigurationBuilder WithPartitionsRevokedHandler(
        Action<IDependencyResolver, List<Confluent.Kafka.TopicPartitionOffset>> partitionsRevokedHandler)
    {
        _partitionRevokedHandlers.Add(partitionsRevokedHandler);
        return this;
    }

    public IConsumerConfigurationBuilder WithStatisticsHandler(Action<string> statisticsHandler)
    {
        _statisticsHandlers.Add(statisticsHandler);
        return this;
    }

    public IConsumerConfigurationBuilder WithStatisticsIntervalMs(int statisticsIntervalMs)
    {
        _statisticsInterval = statisticsIntervalMs;
        return this;
    }

    public IConsumerConfigurationBuilder WithPendingOffsetsStatisticsHandler(
        Action<IDependencyResolver, IEnumerable<Confluent.Kafka.TopicPartitionOffset>> pendingOffsetsHandler,
        TimeSpan interval)
    {
        _pendingOffsetsStatisticsHandlers.Add(new(pendingOffsetsHandler, interval));
        return this;
    }

    public IConsumerConfigurationBuilder WithCustomFactory(ConsumerCustomFactory customFactory)
    {
        _customFactory = customFactory;
        return this;
    }

    public IConsumerConfiguration Build(ClusterConfiguration clusterConfiguration)
    {
        var middlewareConfiguration = _middlewareConfigurationBuilder.Build();

        _consumerConfig ??= new Confluent.Kafka.ConsumerConfig();

        var consumerConfigCopy = new Confluent.Kafka.ConsumerConfig(_consumerConfig.ToDictionary(x => x.Key, x => x.Value));

        consumerConfigCopy.BootstrapServers = _consumerConfig.BootstrapServers ?? string.Join(",", clusterConfiguration.Brokers);
        consumerConfigCopy.GroupId = _consumerConfig.GroupId ?? _groupId;
        consumerConfigCopy.AutoOffsetReset = _consumerConfig.AutoOffsetReset ?? _autoOffsetReset;
        consumerConfigCopy.MaxPollIntervalMs = _consumerConfig.MaxPollIntervalMs ?? _maxPollIntervalMs;
        consumerConfigCopy.StatisticsIntervalMs = _consumerConfig.StatisticsIntervalMs ?? _statisticsInterval;

        consumerConfigCopy.EnableAutoOffsetStore = false;
        consumerConfigCopy.EnableAutoCommit = _consumerConfig.PartitionAssignmentStrategy.IsStopTheWorldStrategy() is false;
        consumerConfigCopy.AutoCommitIntervalMs = _consumerConfig.AutoCommitIntervalMs ?? 5000;

        consumerConfigCopy.ReadSecurityInformationFrom(clusterConfiguration);

        return new ConsumerConfiguration(
            consumerConfigCopy,
            _topics,
            _topicsPartitions,
            _name,
            clusterConfiguration,
            _disableManagement,
            _workersCountCalculator,
            _workersCountEvaluationInterval,
            _bufferSize,
            _workerStopTimeout,
            _distributionStrategyFactory,
            middlewareConfiguration,
            _autoMessageCompletion,
            _noStoreOffsets,
            _initialState,
            _autoCommitInterval,
            _statisticsHandlers,
            _partitionAssignedHandlers,
            _partitionRevokedHandlers,
            _pendingOffsetsStatisticsHandlers,
            _customFactory);
    }
}
