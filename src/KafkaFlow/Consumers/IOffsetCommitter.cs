namespace KafkaFlow.Consumers
{
    using System;
    using Confluent.Kafka;

    public interface IOffsetCommitter : IDisposable
    {
        void StoreOffset(TopicPartitionOffset tpo);
    }
}
