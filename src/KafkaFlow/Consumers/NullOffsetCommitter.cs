namespace KafkaFlow.Consumers
{
    using Confluent.Kafka;

    internal class NullOffsetCommitter : IOffsetCommitter
    {
        public void Dispose()
        {
            // Do nothing
        }

        public void MarkAsProcessed(TopicPartitionOffset tpo)
        {
            // Do nothing
        }
    }
}
