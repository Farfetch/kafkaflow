namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;

    public interface IConsumerConfiguration
    {
        Factory<IDistributionStrategy> DistributionStrategyFactory { get; }

        MiddlewareConfiguration MiddlewareConfiguration { get; }

        IEnumerable<string> Topics { get; }

        string ConsumerName { get; }

        int WorkerCount { get; set; }

        string GroupId { get; }

        int BufferSize { get; }

        bool AutoStoreOffsets { get; }

        TimeSpan AutoCommitInterval { get; }

        IReadOnlyList<Action<string>> StatisticsHandlers { get; }

        IReadOnlyList<Action<IDependencyResolver, List<TopicPartition>>> PartitionsAssignedHandlers { get; }

        IReadOnlyList<Action<IDependencyResolver, List<TopicPartitionOffset>>> PartitionsRevokedHandlers { get; }

        ConsumerCustomFactory CustomFactory { get; }

        ConsumerConfig GetKafkaConfig();
    }
}
