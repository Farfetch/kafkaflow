namespace KafkaFlow.Consumers
{
    using KafkaFlow;

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

        public void CommitProcessedOffsets()
        {
            // Do nothing
        }
    }
}
