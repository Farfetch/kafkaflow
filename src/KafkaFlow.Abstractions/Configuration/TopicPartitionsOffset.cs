using System.Collections.Generic;

namespace KafkaFlow.Configuration;

public class TopicPartitionOffsets
{
    public TopicPartitionOffsets(string name, IDictionary<int, long> partitionOffsets)
    {
        this.Name = name;
        this.PartitionOffsets = partitionOffsets;
    }

    public string Name { get; }

    public IDictionary<int, long> PartitionOffsets { get; }
}
