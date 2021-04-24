namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Confluent.Kafka;
    using KafkaFlow;

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
                    this.committer.StoreOffset(
                        new TopicPartitionOffset(
                            offset.TopicPartition,
                            new Offset(offsets.LastOffset + 1)));
                }
            }
        }

        public void AddOffset(TopicPartitionOffset offset)
        {
            if (this.partitionsOffsets.TryGetValue(
                (offset.Topic, offset.Partition.Value),
                out var offsets))
            {
                offsets.AddOffset(offset.Offset.Value);
            }
        }

        public void RegisterProducerConsumer(
            IProducer<byte[], byte[]> producer,
            IConsumerProducerTransactionCoordinator consumerProducerTransactionCoordinator,
            IConsumerContext consumerContext)
        {
            this.committer.RegisterProducerConsumer(producer, consumerProducerTransactionCoordinator, consumerContext);
        }

        public void Dispose()
        {
            this.committer.Dispose();
        }
    }
}
