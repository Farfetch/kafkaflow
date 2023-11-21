using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace KafkaFlow.Consumers
{
    internal class OffsetManager : IOffsetManager
    {
        private readonly IOffsetCommitter _committer;
        private readonly Dictionary<(string, int), PartitionOffsets> _partitionsOffsets;

        public OffsetManager(
            IOffsetCommitter committer,
            IEnumerable<TopicPartition> partitions)
        {
            _committer = committer;
            _partitionsOffsets = partitions.ToDictionary(
                partition => (partition.Topic, partition.Partition.Value),
                _ => new PartitionOffsets());
        }

        public void MarkAsProcessed(IConsumerContext context)
        {
            if (!_partitionsOffsets.TryGetValue((context.Topic, context.Partition), out var offsets))
            {
                return;
            }

            lock (offsets)
            {
                if (offsets.TryDequeue(context))
                {
                    _committer.MarkAsProcessed(offsets.DequeuedContext.TopicPartitionOffset);
                }
            }
        }

        public void Enqueue(IConsumerContext context)
        {
            if (_partitionsOffsets.TryGetValue(
                    (context.Topic, context.Partition),
                    out var offsets))
            {
                offsets.Enqueue(context);
            }
        }

        public Task WaitContextsCompletionAsync() =>
            Task.WhenAll(
                _partitionsOffsets
                    .Select(x => x.Value.WaitContextsCompletionAsync())
                    .ToList());
    }
}
