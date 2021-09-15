namespace KafkaFlow.Consumers
{
    using Confluent.Kafka;

    internal interface IOffsetManager
    {
        void Commit(TopicPartitionOffset offset);
    }
}
