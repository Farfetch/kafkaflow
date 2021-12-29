namespace KafkaFlow.Client.Protocol.Messages.Implementations.OffsetFetch
{
    using System;
    using KafkaFlow.Client.Protocol.Streams;

    internal class OffsetFetchV5Request : IOffsetFetchRequest
    {
        public OffsetFetchV5Request(string groupId, string topicName, int[] partitions)
        {
            this.GroupId = groupId;
            this.Topics = new[] { this.CreateTopic(topicName, partitions) };
        }

        public ApiKey ApiKey => ApiKey.OffsetFetch;

        public short ApiVersion => 5;

        public Type ResponseType => typeof(OffsetFetchV5Response);

        public string GroupId { get; }

        public IOffsetFetchRequest.ITopic[] Topics { get; }

        void IRequest.Write(MemoryWriter destination)
        {
            destination.WriteString(this.GroupId);
            destination.WriteArray(this.Topics);
        }

        private IOffsetFetchRequest.ITopic CreateTopic(string name, int[] partitions) => new Topic(name, partitions);

        private class Topic : IOffsetFetchRequest.ITopic
        {
            public Topic(string name, int[] partitions)
            {
                this.Name = name;
                this.Partitions = partitions;
            }

            public string Name { get; }

            public int[] Partitions { get; }

            void IRequest.Write(MemoryWriter destination)
            {
                destination.WriteString(this.Name);
                destination.WriteInt32Array(this.Partitions);
            }
        }
    }
}
