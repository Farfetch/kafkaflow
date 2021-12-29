namespace KafkaFlow.Client.Protocol.Messages.Implementations.ListOffsets
{
    using System;
    using System.Linq;
    using KafkaFlow.Client.Protocol.Streams;

    internal class ListOffsetsV5Request : IListOffsetsRequest
    {
        public ListOffsetsV5Request(int replicaId, byte isolationLevel, string topicName, int[] partitions)
        {
            this.ReplicaId = replicaId;
            this.IsolationLevel = isolationLevel;
            this.Topics = new[] { this.CreateTopic(topicName, partitions) };
        }

        public ApiKey ApiKey => ApiKey.ListOffsets;

        public short ApiVersion => 5;

        public Type ResponseType => typeof(ListOffsetsV5Response);

        public int ReplicaId { get; }

        public byte IsolationLevel { get; }

        public IListOffsetsRequest.ITopic[] Topics { get; }

        void IRequest.Write(MemoryWriter destination)
        {
            destination.WriteInt32(this.ReplicaId);
            destination.WriteByte(this.IsolationLevel);
            destination.WriteArray(this.Topics);
        }

        private IListOffsetsRequest.ITopic CreateTopic(string name, int[] partitions) => new Topic(name, partitions);

        private class Topic : IListOffsetsRequest.ITopic
        {
            public Topic(string name, int[] partitions)
            {
                this.Name = name;
                this.Partitions = partitions
                    .Select(p => (IListOffsetsRequest.IPartition)new Partition(p))
                    .ToArray();
            }

            public string Name { get; }

            public IListOffsetsRequest.IPartition[] Partitions { get; }

            void IRequest.Write(MemoryWriter destination)
            {
                destination.WriteString(this.Name);
                destination.WriteArray(this.Partitions);
            }

            private IListOffsetsRequest.IPartition CreatePartition(int partition) => new Partition(partition);
        }

        private class Partition : IListOffsetsRequest.IPartition
        {
            public Partition(int partitionIndex)
            {
                this.PartitionIndex = partitionIndex;
                this.CurrentLeaderEpoch = 0;
                this.Timestamp = -1;
            }

            public int PartitionIndex { get; }

            public int CurrentLeaderEpoch { get; }

            public long Timestamp { get; }

            void IRequest.Write(MemoryWriter destination)
            {
                destination.WriteInt32(this.PartitionIndex);
                destination.WriteInt32(this.CurrentLeaderEpoch);
                destination.WriteInt64(this.Timestamp);
            }
        }
    }
}
