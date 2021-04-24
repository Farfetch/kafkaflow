namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;
    using Acks = KafkaFlow.Acks;

    internal interface IProducerConfiguration
    {
        ClusterConfiguration Cluster { get; }

        string Name { get; }

        string DefaultTopic { get; }

        ProducerConfig BaseProducerConfig { get; }

        Acks? Acks { get; }

        public int TransactionAutoCommitIntervalMs { get; }

        MiddlewareConfiguration MiddlewareConfiguration { get; }

        IReadOnlyList<Action<string>> StatisticsHandlers { get; }

        ProducerCustomFactory CustomFactory { get; }

        ProducerConfig GetKafkaConfig();
    }
}
