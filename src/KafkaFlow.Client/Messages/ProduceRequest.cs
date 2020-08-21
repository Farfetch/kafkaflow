namespace KafkaFlow.Client.Messages
{
    using System.Collections.Concurrent;
    using KafkaFlow.Client.Protocol;
    using KafkaFlow.Client.Protocol.Messages;

    internal class ProduceRequest : IClientRequest<ProduceResponse>
    {
        public ProduceRequest(ProduceAcks acks, int timeout)
        {
            this.Acks = acks;
            this.Timeout = timeout;
        }

        public ApiKey ApiKey => ApiKey.Produce;

        public ProduceAcks Acks { get; }

        public int Timeout { get; }

        public ConcurrentDictionary<string, Topic> Topics { get; } = new ConcurrentDictionary<string, Topic>();

        public class Topic
        {
            public Topic(string name)
            {
                this.Name = name;
            }

            public string Name { get; }

            public ConcurrentDictionary<int, Partition> Partitions { get; } = new ConcurrentDictionary<int, Partition>();
        }

        public class Partition
        {
            public Partition(int id, RecordBatch batch)
            {
                this.Id = id;
                this.Batch = batch;
            }

            public int Id { get; }

            public RecordBatch Batch { get; set; }
        }
    }
}
