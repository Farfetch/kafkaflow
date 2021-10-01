namespace KafkaFlow.Client.Protocol.Messages.Implementations.OffsetFetch
{
    using System;
    using System.Linq;
    using KafkaFlow.Client.Protocol.Streams;

    public class ListOffsetsV5Request : IListOffsetsRequest
    {
        public ListOffsetsV5Request(int replicaId, byte isolationLevel, string topicName, int[] partitions)
        {
            this.ReplicaId = replicaId;
            this.IsolationLevel = isolationLevel;
            this.Topics = new[] { this.CreateTopic(topicName, partitions) };
        }

        public ApiKey ApiKey => ApiKey.ListOffsets;

        public short ApiVersion => 5;

        public int ReplicaId { get; }

        public byte IsolationLevel { get; }

        public IListOffsetsRequest.ITopic[] Topics { get; }

        private IListOffsetsRequest.ITopic CreateTopic(string name, int[] partitions) => new Topic(name, partitions);

        public void Write(MemoryWriter destination)
        {
            destination.WriteInt32(this.ReplicaId);
            destination.WriteByte(this.IsolationLevel);
            destination.WriteArray(this.Topics);
        }

        public class Topic : IListOffsetsRequest.ITopic
        {
            public Topic(string name, int[] partitions)
            {
                this.Name = name;
                this.Partitions = partitions.Select(p => new Partition(p)).ToArray();
            }

            public string Name { get; set; }

            public IListOffsetsRequest.IPartition[] Partitions { get; set; } = Array.Empty<Partition>();

            private IListOffsetsRequest.IPartition CreatePartition(int partition) => new Partition(partition);

            public void Write(MemoryWriter destination)
            {
                destination.WriteString(this.Name);
                destination.WriteArray(this.Partitions);
            }
        }

        public class Partition : IListOffsetsRequest.IPartition
        {
            public Partition(int partitionIndex)
            {
                this.PartitionIndex = partitionIndex;
                this.CurrentLeaderEpoch = 0;
                this.Timestamp = -1;
            }

            public int PartitionIndex { get; set; }

            public int CurrentLeaderEpoch { get; set; }

            public long Timestamp { get; set; }

            public void Write(MemoryWriter destination)
            {
                destination.WriteInt32(this.PartitionIndex);
                destination.WriteInt32(this.CurrentLeaderEpoch);
                destination.WriteInt64(this.Timestamp);
            }
        }

        public Type ResponseType => typeof(ListOffsetsV5Response);
    }
}
