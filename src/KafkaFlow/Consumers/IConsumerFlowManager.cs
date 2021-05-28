namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;

    public interface IConsumerFlowManager : IDisposable
    {
        void Pause(IReadOnlyCollection<TopicPartition> topicPartitions);

        void Resume(IReadOnlyCollection<TopicPartition> topicPartitions);
    }
}
