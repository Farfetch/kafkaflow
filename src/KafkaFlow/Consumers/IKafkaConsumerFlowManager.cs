namespace KafkaFlow.Consumers
{
    using System.Collections.Generic;
    using Confluent.Kafka;

    internal interface IKafkaConsumerFlowManager
    {
        void Pause(IReadOnlyCollection<TopicPartition> topicPartitions);

        void Resume(IReadOnlyCollection<TopicPartition> topicPartitions);
    }
}
