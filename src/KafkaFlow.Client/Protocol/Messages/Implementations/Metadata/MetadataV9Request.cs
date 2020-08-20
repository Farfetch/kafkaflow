namespace KafkaFlow.Client.Protocol.Messages.Implementations.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using KafkaFlow.Client.Protocol.Streams;

    internal class MetadataV9Request : IMetadataRequest, ITaggedFields
    {
        private readonly List<IMetadataRequest.ITopic> topics = new();

        public ApiKey ApiKey => ApiKey.Metadata;

        public short ApiVersion => 9;

        public Type ResponseType => typeof(MetadataV9Response);

        public bool AllowAutoTopicCreation => false;

        public bool IncludeClusterAuthorizedOperations => false;

        public bool IncludeTopicAuthorizedOperations => false;

        public TaggedField[] TaggedFields { get; } = Array.Empty<TaggedField>();

        public IMetadataRequest AddTopic(string topicName)
        {
            this.topics.Add(new Topic { Name = topicName });
            return this;
        }

        public IMetadataRequest AddTopics(IEnumerable<string> topicsNames)
        {
            this.topics.AddRange(topicsNames.Select(topicName => new Topic { Name = topicName }));
            return this;
        }

        void IRequest.Write(MemoryWriter destination)
        {
            destination.WriteCompactArray(this.topics);
            destination.WriteBoolean(this.AllowAutoTopicCreation);
            destination.WriteBoolean(this.IncludeClusterAuthorizedOperations);
            destination.WriteBoolean(this.IncludeTopicAuthorizedOperations);
            destination.WriteTaggedFields(this.TaggedFields);
        }

        private class Topic : IMetadataRequest.ITopic, ITaggedFields
        {
            public string Name { get; set; } = string.Empty;

            public TaggedField[] TaggedFields { get; } = Array.Empty<TaggedField>();

            void IRequest.Write(MemoryWriter destination)
            {
                destination.WriteCompactString(this.Name);
                destination.WriteTaggedFields(this.TaggedFields);
            }
        }
    }
}
