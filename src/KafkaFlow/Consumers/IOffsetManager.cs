namespace KafkaFlow.Consumers
{
    using Confluent.Kafka;

    internal interface IOffsetManager
    {
        void StoreOffset(TopicPartitionOffset offset);
    }
}
