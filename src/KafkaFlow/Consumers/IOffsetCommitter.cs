namespace KafkaFlow.Consumers
{
    using System;

    internal interface IOffsetCommitter : IDisposable
    {
        void MarkAsProcessed(TopicPartitionOffset tpo);
    }
}
