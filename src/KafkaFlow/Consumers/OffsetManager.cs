namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Confluent.Kafka;

    internal class OffsetManager : IOffsetManager, IDisposable
    {
        private readonly IOffsetCommitter committer;
        private readonly Dictionary<(string, int), PartitionOffsets> partitionsOffsets;

        public OffsetManager(
            IOffsetCommitter committer,
            IEnumerable<TopicPartition> partitions)
        {
            this.committer = committer;
            this.partitionsOffsets = partitions.ToDictionary(
                partition => (partition.Topic, partition.Partition.Value),
                partition => new PartitionOffsets());
        }

        public void Commit(TopicPartitionOffset offset)
        {
            if (!this.partitionsOffsets.TryGetValue((offset.Topic, offset.Partition.Value), out var offsets))
            {
                return;
            }

            lock (offsets)
            {
                if (offsets.ShouldCommit(offset.Offset.Value, out var lastProcessedOffset))
                {
                    this.committer.Commit(
                        new TopicPartitionOffset(
                            offset.TopicPartition,
                            new Offset(lastProcessedOffset + 1)));
                }
            }
        }

        public void Enqueue(TopicPartitionOffset offset)
        {
            if (this.partitionsOffsets.TryGetValue(
                (offset.Topic, offset.Partition.Value),
                out var offsets))
            {
                offsets.Enqueue(offset.Offset.Value);
            }
        }

        public void Dispose()
        {
            this.committer.Dispose();
        }
    }
}
