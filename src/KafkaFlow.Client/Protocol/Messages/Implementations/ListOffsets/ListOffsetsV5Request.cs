namespace KafkaFlow.Client.Protocol.Messages.Implementations.ListOffsets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using KafkaFlow.Client.Protocol.Streams;

    internal class ListOffsetsV5Request : IListOffsetsRequest
    {
        private const int ReplicaId = -1;
        private const byte IsolationLevel = 0;

        private readonly List<Topic> topics = new();

        public ApiKey ApiKey => ApiKey.ListOffsets;

        public short ApiVersion => 5;

        public Type ResponseType => typeof(ListOffsetsV5Response);

        public IListOffsetsRequest AddTopic(string name, IEnumerable<int> partitions)
        {
            this.topics.Add(new Topic(name, partitions));
            return this;
        }

        void IRequest.Write(MemoryWriter destination)
        {
            destination.WriteInt32(ReplicaId);
            destination.WriteByte(IsolationLevel);
            destination.WriteArray(this.topics);
        }

        private class Topic : IRequest
        {
            private readonly string name;
            private readonly IReadOnlyList<Partition> partitions;

            public Topic(string name, IEnumerable<int> partitions)
            {
                this.name = name;
                this.partitions = partitions
                    .Select(p => new Partition(p))
                    .ToList();
            }

            void IRequest.Write(MemoryWriter destination)
            {
                destination.WriteString(this.name);
                destination.WriteArray(this.partitions);
            }
        }

        private class Partition : IRequest
        {
            private const int CurrentLeaderEpoch = 0;
            private const long Timestamp = -1;
            private readonly int id;

            public Partition(int id) => this.id = id;

            void IRequest.Write(MemoryWriter destination)
            {
                destination.WriteInt32(this.id);
                destination.WriteInt32(CurrentLeaderEpoch);
                destination.WriteInt64(Timestamp);
            }
        }
    }
}
