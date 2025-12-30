using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KafkaFlow.Configuration;

internal class ConsumerConfiguration : IConsumerConfiguration
{
    private readonly Confluent.Kafka.ConsumerConfig _consumerConfig;

    public ConsumerConfiguration(
        Confluent.Kafka.ConsumerConfig consumerConfig,
        IReadOnlyList<string> topics,
        IReadOnlyList<TopicPartitions> manualAssignPartitions,
        IReadOnlyList<TopicPartitionOffsets> manualAssignPartitionOffsets,
        string consumerName,
        ClusterConfiguration clusterConfiguration,
        bool managementDisabled,
        Func<WorkersCountContext, IDependencyResolver, Task<int>> workersCountCalculator,
        TimeSpan workersCountEvaluationInterval,
        int bufferSize,
        TimeSpan workerStopTimeout,
        Factory<IWorkerDistributionStrategy> distributionStrategyFactory,
        IReadOnlyList<MiddlewareConfiguration> middlewaresConfigurations,
        bool autoMessageCompletion,
        bool noStoreOffsets,
        ConsumerInitialState initialState,
        TimeSpan autoCommitInterval,
        IReadOnlyList<Action<string>> statisticsHandlers,
        IReadOnlyList<Action<IDependencyResolver, List<Confluent.Kafka.TopicPartition>>> partitionsAssignedHandlers,
        IReadOnlyList<Action<IDependencyResolver, List<Confluent.Kafka.TopicPartitionOffset>>> partitionsRevokedHandlers,
        IReadOnlyList<PendingOffsetsStatisticsHandler> pendingOffsetsStatisticsHandlers,
        ConsumerCustomFactory customFactory)
    {
        _consumerConfig = consumerConfig ?? throw new ArgumentNullException(nameof(consumerConfig));

        if (string.IsNullOrEmpty(_consumerConfig.GroupId))
        {
            throw new ArgumentNullException(nameof(consumerConfig.GroupId));
        }

        this.DistributionStrategyFactory =
            distributionStrategyFactory ?? throw new ArgumentNullException(nameof(distributionStrategyFactory));
        this.MiddlewaresConfigurations =
            middlewaresConfigurations ?? throw new ArgumentNullException(nameof(middlewaresConfigurations));
        this.AutoMessageCompletion = autoMessageCompletion;
        this.NoStoreOffsets = noStoreOffsets;
        this.InitialState = initialState;
        this.AutoCommitInterval = autoCommitInterval;
        this.Topics = topics ?? throw new ArgumentNullException(nameof(topics));
        this.ManualAssignPartitions = manualAssignPartitions ?? throw new ArgumentNullException(nameof(manualAssignPartitions));
        this.ManualAssignPartitionOffsets = manualAssignPartitionOffsets ?? throw new ArgumentNullException(nameof(manualAssignPartitionOffsets));
        this.ConsumerName = consumerName ?? Guid.NewGuid().ToString();
        this.ClusterConfiguration = clusterConfiguration;
        this.ManagementDisabled = managementDisabled;
        this.WorkersCountCalculator = workersCountCalculator;
        this.WorkersCountEvaluationInterval = workersCountEvaluationInterval;
        this.WorkerStopTimeout = workerStopTimeout;
        this.StatisticsHandlers = statisticsHandlers;
        this.PartitionsAssignedHandlers = partitionsAssignedHandlers;
        this.PartitionsRevokedHandlers = partitionsRevokedHandlers;
        this.PendingOffsetsStatisticsHandlers = pendingOffsetsStatisticsHandlers;
        this.CustomFactory = customFactory;

        this.BufferSize = bufferSize > 0
            ? bufferSize
            : throw new ArgumentOutOfRangeException(
                nameof(bufferSize),
                bufferSize,
                "The value must be greater than 0");
    }

    public Factory<IWorkerDistributionStrategy> DistributionStrategyFactory { get; }

    public IReadOnlyList<MiddlewareConfiguration> MiddlewaresConfigurations { get; }

    public IReadOnlyList<string> Topics { get; }

    public IReadOnlyList<TopicPartitions> ManualAssignPartitions { get; }

    public IReadOnlyList<TopicPartitionOffsets> ManualAssignPartitionOffsets { get; }

    public string ConsumerName { get; }

    public ClusterConfiguration ClusterConfiguration { get; }

    public bool ManagementDisabled { get; }

    public Func<WorkersCountContext, IDependencyResolver, Task<int>> WorkersCountCalculator { get; set; }

    public TimeSpan WorkersCountEvaluationInterval { get; }

    public string GroupId => _consumerConfig.GroupId;

    public int BufferSize { get; }

    public TimeSpan WorkerStopTimeout { get; }

    public bool AutoMessageCompletion { get; }

    public bool NoStoreOffsets { get; }

    public ConsumerInitialState InitialState { get; }

    public TimeSpan AutoCommitInterval { get; }

    public IReadOnlyList<Action<string>> StatisticsHandlers { get; }

    public IReadOnlyList<Action<IDependencyResolver, List<Confluent.Kafka.TopicPartition>>> PartitionsAssignedHandlers { get; }

    public IReadOnlyList<Action<IDependencyResolver, List<Confluent.Kafka.TopicPartitionOffset>>> PartitionsRevokedHandlers { get; }

    public IReadOnlyList<PendingOffsetsStatisticsHandler> PendingOffsetsStatisticsHandlers { get; }

    public ConsumerCustomFactory CustomFactory { get; }

    public Confluent.Kafka.ConsumerConfig GetKafkaConfig()
    {
        return _consumerConfig;
    }
}
