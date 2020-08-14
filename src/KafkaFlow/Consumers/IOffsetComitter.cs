namespace KafkaFlow.Consumers
{
    using System;
    using Confluent.Kafka;

    public interface IOffsetComitter : IDisposable
    {
        void StoreOffset(TopicPartitionOffset tpo);
    }
}
