namespace KafkaFlow.Consumers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Confluent.Kafka;

    internal class OffsetManager : IOffsetManager
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
                _ => new PartitionOffsets());
        }

        public void MarkAsProcessed(IConsumerContext context)
        {
            if (!this.partitionsOffsets.TryGetValue((context.Topic, context.Partition), out var offsets))
            {
                return;
            }

            lock (offsets)
            {
                if (offsets.TryDequeue(context))
                {
                    this.committer.MarkAsProcessed(offsets.DequeuedContext.TopicPartitionOffset);
                }
            }
        }

        public void Enqueue(IConsumerContext context)
        {
            if (this.partitionsOffsets.TryGetValue(
                    (context.Topic, context.Partition),
                    out var offsets))
            {
                offsets.Enqueue(context);
            }
        }

        public Task WaitContextsCompletionAsync() =>
            Task.WhenAll(
                this.partitionsOffsets
                    .Select(x => x.Value.WaitContextsCompletionAsync())
                    .ToList());
    }
}
