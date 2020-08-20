namespace KafkaFlow.Client.Protocol.Messages.Implementations.OffsetFetch
{
    using System;
    using KafkaFlow.Client.Protocol.Streams;

    /// <summary>
    /// Request message to achieve the list of committed offsets in a topic by a consumer group
    /// </summary>
    public class OffsetFetchV5Request : IOffsetFetchRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OffsetFetchV5Request"/> class.
        /// </summary>
        /// <param name="groupId">The group identifier</param>
        /// <param name="topicName">The topic name</param>
        /// <param name="partitions">The partitions list</param>
        public OffsetFetchV5Request(string groupId, string topicName, int[] partitions)
        {
            this.GroupId = groupId;
            this.Topics = new[] { this.CreateTopic(topicName, partitions) };
        }

        /// <inheritdoc/>
        public ApiKey ApiKey => ApiKey.OffsetFetch;

        /// <inheritdoc/>
        public short ApiVersion => 5;

        /// <inheritdoc/>
        public Type ResponseType => typeof(OffsetFetchV5Response);

        /// <inheritdoc/>
        public string GroupId { get; }

        /// <inheritdoc/>
        public IOffsetFetchRequest.ITopic[] Topics { get; }

        /// <inheritdoc/>
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
