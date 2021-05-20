namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;

    internal interface IConsumerConfiguration
    {
        Factory<IDistributionStrategy> DistributionStrategyFactory { get; }

        MiddlewareConfiguration MiddlewareConfiguration { get; }

        IEnumerable<string> Topics { get; }

        string ConsumerName { get; }

        bool IsReadonly { get; }

        int WorkersCount { get; set; }

        string GroupId { get; }

        int BufferSize { get; }

        bool AutoStoreOffsets { get; }

        TimeSpan AutoCommitInterval { get; }

        IReadOnlyList<Action<string>> StatisticsHandlers { get; }


        IReadOnlyList<Action<IDependencyResolver, List<TopicPartition>>> PartitionsAssignedHandlers { get; }

        ConsumerCustomFactory CustomFactory { get; }

        ConsumerConfig GetKafkaConfig();
    }
}
