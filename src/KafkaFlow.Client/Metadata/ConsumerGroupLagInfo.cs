namespace KafkaFlow.Client.Metadata
{
    using System.Collections.Generic;

    public class ConsumerGroupLagInfo
    {
        public ConsumerGroupLagInfo(IReadOnlyList<ConsumerGroupLagInfo.Topic> topics)
        {
            this.Topics = topics;
        }

        public IReadOnlyList<ConsumerGroupLagInfo.Topic> Topics { get; }

        public class Topic
        {
            public Topic(string name, IReadOnlyList<Partition> partitions)
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
