namespace KafkaFlow
{
    using System.Collections.Generic;

    public record TopicMetadata
    {
        public TopicMetadata(string name, IReadOnlyCollection<TopicPartitionMetadata> partitions)
        {
            this.Name = name;
            this.Partitions = partitions;
        }

        public string Name { get; }

        public IReadOnlyCollection<TopicPartitionMetadata> Partitions { get; }
    }
}
