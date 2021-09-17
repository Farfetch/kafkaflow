namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Confluent.Kafka;

    internal class OffsetManager : IOffsetManager, IDisposable
    {
        private readonly Dictionary<TopicPartitionOffset, List<Action>> onProcessedActions = new();

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

            this.ExecuteOffsetActions(offset);

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
        }

        public void OnOffsetProcessed(TopicPartitionOffset offset, Action action)
        {
            this.onProcessedActions
                .GetOrAdd(offset, _ => new List<Action>())
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

            this.onProcessedActions.Remove(offset);

            foreach (var action in actions)
            {
                action();
            }
        }
    }
}
