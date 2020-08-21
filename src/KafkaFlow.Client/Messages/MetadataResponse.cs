namespace KafkaFlow.Client.Messages
{
    internal class MetadataResponse : IClientResponse
    {
        public MetadataResponse(Broker[] brokers, string? clusterId, int controllerId, Topic[] topics)
        {
            this.Brokers = brokers;
            this.ClusterId = clusterId;
            this.ControllerId = controllerId;
            this.Topics = topics;
        }

        public Broker[] Brokers { get; }

        public string? ClusterId { get; }

        public int ControllerId { get; }

        public Topic[] Topics { get; }

        public class Topic
        {
            public Topic(string name, Partition[] partitions)
            {
                this.Name = name;
                this.Partitions = partitions;
            }

            public string Name { get; }
            public Partition[] Partitions { get; }
        }

        public class Partition
        {
            public Partition(int id, int leaderId)
            {
                this.Id = id;
                this.LeaderId = leaderId;
            }

            public int Id { get; }
            public int LeaderId { get; }
        }

        public class Broker
        {
            public Broker(int nodeId, string host, int port)
            {
                this.NodeId = nodeId;
                this.Host = host;
                this.Port = port;
            }

            public int NodeId { get; }
            public string Host { get; }
            public int Port { get; }
        }
    }
}
