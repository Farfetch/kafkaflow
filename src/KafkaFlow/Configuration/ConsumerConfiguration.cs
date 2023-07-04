namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;

    internal class ConsumerConfiguration : IConsumerConfiguration
    {
        private readonly ConsumerConfig consumerConfig;

        public ConsumerConfiguration(
            ConsumerConfig consumerConfig,
            IReadOnlyList<string> topics,
            IReadOnlyList<TopicPartitions> manualAssignPartitions,
            string consumerName,
            ClusterConfiguration clusterConfiguration,
            bool managementDisabled,
            Func<WorkersCountContext, int> workersCountCalculator,
            int bufferSize,
            TimeSpan workerStopTimeout,
            Factory<IDistributionStrategy> distributionStrategyFactory,
            IReadOnlyList<MiddlewareConfiguration> middlewaresConfigurations,
            bool autoStoreOffsets,
            bool noStoreOffsets,
            ConsumerInitialState initialState,
            TimeSpan autoCommitInterval,
            IReadOnlyList<Action<string>> statisticsHandlers,
            IReadOnlyList<Action<IDependencyResolver, List<TopicPartition>>> partitionsAssignedHandlers,
            IReadOnlyList<Action<IDependencyResolver, List<TopicPartitionOffset>>> partitionsRevokedHandlers,
            IReadOnlyList<(Action<IDependencyResolver, IEnumerable<TopicPartitionOffset>> handler, TimeSpan interval)>
                pendingOffsetsHandlers,
            ConsumerCustomFactory customFactory)
        {
            this.consumerConfig = consumerConfig ?? throw new ArgumentNullException(nameof(consumerConfig));

            if (string.IsNullOrEmpty(this.consumerConfig.GroupId))
            {
                throw new ArgumentNullException(nameof(consumerConfig.GroupId));
            }

            this.DistributionStrategyFactory =
                distributionStrategyFactory ?? throw new ArgumentNullException(nameof(distributionStrategyFactory));
            this.MiddlewaresConfigurations =
                middlewaresConfigurations ?? throw new ArgumentNullException(nameof(middlewaresConfigurations));
            this.AutoStoreOffsets = autoStoreOffsets;
            this.NoStoreOffsets = noStoreOffsets;
            this.InitialState = initialState;
            this.AutoCommitInterval = autoCommitInterval;
            this.Topics = topics ?? throw new ArgumentNullException(nameof(topics));
            this.ManualAssignPartitions = manualAssignPartitions ?? throw new ArgumentNullException(nameof(manualAssignPartitions));
            this.ConsumerName = consumerName ?? Guid.NewGuid().ToString();
            this.ClusterConfiguration = clusterConfiguration;
            this.ManagementDisabled = managementDisabled;
            this.WorkersCountCalculator = workersCountCalculator;
            this.WorkerStopTimeout = workerStopTimeout;
            this.StatisticsHandlers = statisticsHandlers;
            this.PartitionsAssignedHandlers = partitionsAssignedHandlers;
            this.PartitionsRevokedHandlers = partitionsRevokedHandlers;
            this.PendingOffsetsHandlers = pendingOffsetsHandlers;
            this.CustomFactory = customFactory;

            this.BufferSize = bufferSize > 0 ?
                bufferSize :
                throw new ArgumentOutOfRangeException(
                    nameof(bufferSize),
                    bufferSize,
                    "The value must be greater than 0");
        }

        public Factory<IDistributionStrategy> DistributionStrategyFactory { get; }

        public IReadOnlyList<MiddlewareConfiguration> MiddlewaresConfigurations { get; }

        public IReadOnlyList<string> Topics { get; }

        public IReadOnlyList<TopicPartitions> ManualAssignPartitions { get; }

        public string ConsumerName { get; }

        public ClusterConfiguration ClusterConfiguration { get; }

        public bool ManagementDisabled { get; }

        public Func<WorkersCountContext, int> WorkersCountCalculator { get; set; }

        public string GroupId => this.consumerConfig.GroupId;

        public int BufferSize { get; }

        public TimeSpan WorkerStopTimeout { get; }

        public bool AutoStoreOffsets { get; }

        public bool NoStoreOffsets { get; }

        public ConsumerInitialState InitialState { get; }

        public TimeSpan AutoCommitInterval { get; }

        public IReadOnlyList<Action<string>> StatisticsHandlers { get; }

        public IReadOnlyList<Action<IDependencyResolver, List<TopicPartition>>> PartitionsAssignedHandlers { get; }

        public IReadOnlyList<Action<IDependencyResolver, List<TopicPartitionOffset>>> PartitionsRevokedHandlers { get; }

        public IReadOnlyList<(Action<IDependencyResolver, IEnumerable<TopicPartitionOffset>> handler, TimeSpan interval)>
            PendingOffsetsHandlers { get; }

        public ConsumerCustomFactory CustomFactory { get; }

        public ConsumerConfig GetKafkaConfig()
        {
            return this.consumerConfig;
        }
    }
}
