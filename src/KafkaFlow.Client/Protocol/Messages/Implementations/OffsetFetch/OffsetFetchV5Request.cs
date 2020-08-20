namespace KafkaFlow.Client.Protocol.Messages.Implementations.OffsetFetch
{
    using System;
    using System.Collections.Generic;
    using KafkaFlow.Client.Protocol.Streams;

    internal class OffsetFetchV5Request : IOffsetFetchRequest
    {
        private readonly List<Topic> topics = new();

        public OffsetFetchV5Request(string groupId)
        {
            this.GroupId = groupId;
        }

        public ApiKey ApiKey => ApiKey.OffsetFetch;

        public short ApiVersion => 5;

        public Type ResponseType => typeof(OffsetFetchV5Response);

        public string GroupId { get; }

        public void AddTopic(string name, IReadOnlyList<int> partitions) => this.topics.Add(new Topic(name, partitions));

        void IRequest.Write(MemoryWriter destination)
        {
            destination.WriteString(this.GroupId);
            destination.WriteArray(this.topics);
        }

        private class Topic : IRequest
        {
            private readonly string name;
            private readonly IReadOnlyList<int> partitions;

            public Topic(string name, IReadOnlyList<int> partitions)
            {
                this.name = name;
                this.partitions = partitions;
            }

            void IRequest.Write(MemoryWriter destination)
            {
                destination.WriteString(this.name);
                destination.WriteInt32Array(this.partitions);
            }
        }
    }
}
