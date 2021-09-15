namespace KafkaFlow.Consumers
{
    using System;
    using Confluent.Kafka;

    internal interface IOffsetCommitter : IDisposable
    {
        void Commit(TopicPartitionOffset tpo);
    }
}
