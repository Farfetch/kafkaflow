using System.Collections.Generic;

namespace KafkaFlow;

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
