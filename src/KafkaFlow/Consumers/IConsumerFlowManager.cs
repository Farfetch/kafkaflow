namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;

    internal interface IConsumerFlowManager : IDisposable
    {
        ConsumerFlowStatus Status { get; }

        void Pause(IReadOnlyCollection<TopicPartition> topicPartitions);

        void Resume(IReadOnlyCollection<TopicPartition> topicPartitions);
    }
}
