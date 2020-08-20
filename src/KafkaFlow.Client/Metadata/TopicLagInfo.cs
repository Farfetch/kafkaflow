namespace KafkaFlow.Client.Metadata
{
    using System.Collections.Generic;

    public class TopicLagInfo
    {
        public TopicLagInfo(IReadOnlyList<ConsumerGroup> consumerGroups)
        {
            this.ConsumerGroups = consumerGroups;
        }

        public IReadOnlyList<ConsumerGroup> ConsumerGroups { get; }

        public class ConsumerGroup
        {
            public ConsumerGroup(string name, IReadOnlyList<Partition> partitions)
            {
                this.Name = name;
                this.Partitions = partitions;
            }

            public string Name { get; }

            public IReadOnlyList<Partition> Partitions { get; }
        }

        public class Partition
        {
            public Partition(int id, long lag)
            {
                this.Id = id;
                this.Lag = lag;
            }

            public int Id { get; }

            public long Lag { get; }
        }
    }
}
