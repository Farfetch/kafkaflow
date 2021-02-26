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

        int WorkerCount { get; set; }

        string GroupId { get; }

        int BufferSize { get; }

        bool AutoStoreOffsets { get; }

        TimeSpan AutoCommitInterval { get; }

        IReadOnlyList<Action<string>> StatisticsHandlers { get; }

        ConsumerCustomFactory CustomFactory { get; }

        ConsumerConfig GetKafkaConfig();
    }
}
