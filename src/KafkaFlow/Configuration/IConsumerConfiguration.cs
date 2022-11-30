namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;

    /// <summary>
    /// Represents the Consumer configuration values
    /// </summary>
    public interface IConsumerConfiguration
    {
        /// <summary>
        /// Gets the consumer worker distribution strategy
        /// </summary>
        Factory<IDistributionStrategy> DistributionStrategyFactory { get; }

        /// <summary>
        /// Gets the consumer middlewares configurations
        /// </summary>
        IReadOnlyList<MiddlewareConfiguration> MiddlewaresConfigurations { get; }

        /// <summary>
        /// Gets the consumer configured topics
        /// </summary>
        IReadOnlyList<string> Topics { get; }

        /// <summary>
        /// Gets the topic partitions to manually assign
        /// </summary>
        IReadOnlyList<TopicPartitions> ManualAssignPartitions { get; }

        /// <summary>
        /// Gets the consumer name
        /// </summary>
        string ConsumerName { get; }

        /// <summary>
        /// Gets the cluster configuration
        /// </summary>
        ClusterConfiguration ClusterConfiguration { get; }

        /// <summary>
        /// Gets a value indicating whether the consumer is able to be manageable or not
        /// </summary>
        bool ManagementDisabled { get; }

        /// <summary>
        /// Gets or sets the number of workers
        /// </summary>
        int WorkersCount { get; set; }

        /// <summary>
        /// Gets the consumer group
        /// </summary>
        string GroupId { get; }

        /// <summary>
        /// Gets the buffer size used for each worker
        /// </summary>
        int BufferSize { get; }

        /// <summary>
        /// Gets the time that the worker will wait to process the buffered messages
        /// before canceling the <see cref="IConsumerContext.WorkerStopped"/>
        /// </summary>
        TimeSpan WorkerStopTimeout { get; }

        /// <summary>
        /// Gets a value indicating whether if the application should store store at the end
        /// </summary>
        bool AutoStoreOffsets { get; }

        /// <summary>
        /// Gets a value indicating that no offsets will be stored on Kafka
        /// </summary>
        bool NoStoreOffsets { get; }

        /// <summary>
        /// Gets the interval between commits
        /// </summary>
        TimeSpan AutoCommitInterval { get; }

        /// <summary>
        /// Gets the handlers used to collects statistics
        /// </summary>
        IReadOnlyList<Action<string>> StatisticsHandlers { get; }

        /// <summary>
        /// Gets the handlers that will be called when the partitions are assigned
        /// </summary>
        IReadOnlyList<Action<IDependencyResolver, List<TopicPartition>>> PartitionsAssignedHandlers { get; }

        /// <summary>
        /// Gets the handlers that will be called when the partitions are revoked
        /// </summary>
        IReadOnlyList<Action<IDependencyResolver, List<TopicPartitionOffset>>> PartitionsRevokedHandlers { get; }

        /// <summary>
        /// Gets the handlers that will be called when there are pending offsets
        /// </summary>
        IReadOnlyList<(Action<IDependencyResolver, IEnumerable<TopicPartitionOffset>> handler, TimeSpan interval)> PendingOffsetsHandlers
        {
            get;
        }

        /// <summary>
        /// Gets the custom factory used to create a new <see cref="KafkaFlow.Consumers.IConsumer"/>
        /// </summary>
        ConsumerCustomFactory CustomFactory { get; }

        /// <summary>
        /// Gets the consumer initial state
        /// </summary>
        ConsumerInitialState InitialState { get; }

        /// <summary>
        /// Parses KafkaFlow configuration to Confluent configuration
        /// </summary>
        /// <returns></returns>
        ConsumerConfig GetKafkaConfig();
    }
}
