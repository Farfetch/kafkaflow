namespace KafkaFlow.Client.Protocol.Messages
{
    using System;
    using System.IO;

    public class TopicMetadataV9Request : IRequestMessageV2<TopicMetadataV9Response>
    {
        public ApiKey ApiKey => ApiKey.Metadata;

        public short ApiVersion => 9;

        public Topic[] Topics { get; }

        public bool AllowAutoTopicCreation { get; }

        public bool IncludeClusterAuthorizedOperations { get; }

        public bool IncludeTopicAuthorizedOperations { get; }

        public TaggedField[] TaggedFields => Array.Empty<TaggedField>();

        public TopicMetadataV9Request(
            Topic[] topics,
            bool allowAutoTopicCreation,
            bool includeClusterAuthorizedOperations,
            bool includeTopicAuthorizedOperations)
        {
            this.Topics = topics;
            this.AllowAutoTopicCreation = allowAutoTopicCreation;
            this.IncludeClusterAuthorizedOperations = includeClusterAuthorizedOperations;
            this.IncludeTopicAuthorizedOperations = includeTopicAuthorizedOperations;
        }

        public void Write(Stream destination)
        {
            destination.WriteCompactArray(this.Topics);
            destination.WriteBoolean(this.AllowAutoTopicCreation);
            destination.WriteBoolean(this.IncludeClusterAuthorizedOperations);
            destination.WriteBoolean(this.IncludeTopicAuthorizedOperations);
            destination.WriteTaggedFields(this.TaggedFields);
        }

        public class Topic : IRequestV2
        {
            public Topic(string name)
            {
                this.Name = name;
            }

            public string Name { get; }

            public TaggedField[] TaggedFields => Array.Empty<TaggedField>();

            public void Write(Stream destination)
            {
                destination.WriteCompactString(this.Name);
                destination.WriteTaggedFields(this.TaggedFields);
            }
        }
    }
}
