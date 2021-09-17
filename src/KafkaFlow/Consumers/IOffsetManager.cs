namespace KafkaFlow.Consumers
{
    using System;
    using Confluent.Kafka;

    internal interface IOffsetManager
    {
        void MarkAsProcessed(TopicPartitionOffset offset);

        void OnOffsetProcessed(TopicPartitionOffset offset, Action action);
    }
}
