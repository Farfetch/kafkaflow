namespace KafkaFlow
{
    using System.Collections.Generic;

    public record TopicMetadata
    {
        public TopicMetadata(IReadOnlyCollection<TopicPartitionMetadata> partitions)
        {
            this.Partitions = partitions;
        }

        public IReadOnlyCollection<TopicPartitionMetadata> Partitions { get; }
    }
}
