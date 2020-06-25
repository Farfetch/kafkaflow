namespace KafkaFlow.Consumers
{
    using System.Collections.Generic;
    using System.Linq;
    using Confluent.Kafka;

    public class OffsetManager : IOffsetManager
    {
        private readonly IConsumer<byte[], byte[]> consumer;
        private readonly Dictionary<(string, int), PartitionOffsets> partitionsOffsets;

        public OffsetManager(
            IConsumer<byte[], byte[]> consumer,
            IEnumerable<TopicPartition> partitions)
        {
            this.consumer = consumer;

            this.partitionsOffsets = partitions.ToDictionary(
                partition => (partition.Topic, partition.Partition.Value),
                partition => new PartitionOffsets());
        }

        public void StoreOffset(TopicPartitionOffset offset)
        {
            if (!this.partitionsOffsets.TryGetValue((offset.Topic, offset.Partition.Value), out var offsets))
            {
                return;
            }

            lock (offsets)
            {
                if (offsets.ShouldUpdateOffset(offset.Offset.Value))
                {
                    this.consumer.StoreOffset(
                        new TopicPartitionOffset(
                            offset.TopicPartition,
                            new Offset(offsets.LastOffset + 1)));
                }
            }
        }

        public void InitializeOffsetIfNeeded(TopicPartitionOffset partitionOffset)
        {
            if (
                this.partitionsOffsets.TryGetValue((partitionOffset.Topic, partitionOffset.Partition.Value), out var offsets) &&
                offsets.LastOffset == Offset.Unset)
            {
                offsets.InitializeLastOffset(partitionOffset.Offset.Value - 1);
            }
        }
    }
}
