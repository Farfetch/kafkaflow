namespace KafkaFlow.Consumers
{
    using System;
    using Confluent.Kafka;

    internal interface IOffsetCommitter : IDisposable
    {
        void MarkAsProcessed(TopicPartitionOffset tpo);
    }
}
