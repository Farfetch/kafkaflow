namespace KafkaFlow.Client.Protocol.Messages.Implementations.Metadata
{
    using System;
    using System.Linq;
    using KafkaFlow.Client.Protocol.Streams;

    /// <summary>
    /// Request message to achieve metadata of a topics list
    /// </summary>
    public class MetadataV9Request : IMetadataRequest, ITaggedFields
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataV9Request"/> class.
        /// </summary>
        /// <param name="topics">List of topics</param>
        public MetadataV9Request(params string[] topics)
        {
            this.CreateTopics(topics);
        }

        /// <inheritdoc/>
        public ApiKey ApiKey => ApiKey.Metadata;

        /// <inheritdoc/>
        public short ApiVersion => 9;

        /// <inheritdoc/>
        public Type ResponseType => typeof(MetadataV9Response);

        /// <inheritdoc/>
        public IMetadataRequest.ITopic[] Topics { get; private set; }

        /// <inheritdoc/>
        public bool AllowAutoTopicCreation { get; }

        /// <inheritdoc/>
        public bool IncludeClusterAuthorizedOperations { get; }

        /// <inheritdoc/>
        public bool IncludeTopicAuthorizedOperations { get; }

        /// <inheritdoc/>
        public TaggedField[] TaggedFields { get; } = Array.Empty<TaggedField>();

        /// <inheritdoc/>
        void IRequest.Write(MemoryWriter destination)
        {
            destination.WriteCompactArray(this.Topics);
            destination.WriteBoolean(this.AllowAutoTopicCreation);
            destination.WriteBoolean(this.IncludeClusterAuthorizedOperations);
            destination.WriteBoolean(this.IncludeTopicAuthorizedOperations);
            destination.WriteTaggedFields(this.TaggedFields);
        }

        private void CreateTopics(string[] topics)
        {
            this.Topics = topics?.Select(t => new Topic(t)).ToArray() ?? Array.Empty<IMetadataRequest.ITopic>();
        }

        private class Topic : IMetadataRequest.ITopic, ITaggedFields
        {
            public Topic(string topicName)
            {
                this.Name = topicName;
            }

            public string Name { get; }

            public TaggedField[] TaggedFields { get; } = Array.Empty<TaggedField>();

            void IRequest.Write(MemoryWriter destination)
            {
                destination.WriteCompactString(this.Name);
                destination.WriteTaggedFields(this.TaggedFields);
            }
        }
    }
}
