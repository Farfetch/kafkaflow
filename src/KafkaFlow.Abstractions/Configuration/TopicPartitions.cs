using System.Collections.Generic;

namespace KafkaFlow.Configuration
{
    public class TopicPartitions
    {
        public TopicPartitions(string name, IEnumerable<int> partitions)
        {
            this.Name = name;
            this.Partitions = partitions;
        }

        public string Name { get; }

        public IEnumerable<int> Partitions { get; }
    }
}
