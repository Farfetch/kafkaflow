namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Confluent.Kafka;

    internal class OffsetManager : IOffsetManager, IDisposable
    {
        private readonly ConcurrentDictionary<TopicPartitionOffset, ConcurrentBag<Action>> onProcessedActions = new();

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

        public void MarkAsProcessed(TopicPartitionOffset offset)
        {
            if (!this.partitionsOffsets.TryGetValue((offset.Topic, offset.Partition.Value), out var offsets))
            {
                return;
            }

            lock (offsets)
            {
                if (offsets.ShouldCommit(offset.Offset.Value, out var lastProcessedOffset))
                {
                    this.committer.MarkAsProcessed(
                        new TopicPartitionOffset(
                            offset.TopicPartition,
                            new Offset(lastProcessedOffset + 1)));
                }
            }

            this.ExecuteOffsetActions(offset);
        }

        public void OnOffsetProcessed(TopicPartitionOffset offset, Action action)
        {
            this.onProcessedActions
                .SafeGetOrAdd(offset, _ => new())
                .Add(action);
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

        private void ExecuteOffsetActions(TopicPartitionOffset offset)
        {
            if (!this.onProcessedActions.TryGetValue(offset, out var actions))
            {
                return;
            }

            this.onProcessedActions.TryRemove(offset, out _);

            foreach (var action in actions)
            {
                action();
            }
        }
    }
}
