namespace KafkaFlow.Client.Protocol.Messages
{
    using System;
    using KafkaFlow.Client.Protocol.Messages.Implementations.Metadata;
    using KafkaFlow.Client.Protocol.Messages.Implementations.OffsetFetch;

    public class RequestFactory : IRequestFactory
    {
        private readonly IBrokerCapabilities capabilities;

        public RequestFactory(IBrokerCapabilities capabilities)
        {
            this.capabilities = capabilities;
        }

        public IMetadataRequest CreateMetadata()
        {
            return this.capabilities.GetVersionRange(ApiKey.Metadata) switch
            {
                { Min: <= 9, Max: >= 9 } => new MetadataV9Request(),
                _ => throw new Exception()
            };
        }

        public IOffsetFetchRequest CreateOffsetFetch(string groupId, string topicName, int[] partitions)
        {
            return this.capabilities.GetVersionRange(ApiKey.OffsetFetch) switch
            {
                { Min: <= 5, Max: >= 5 } => new OffsetFetchV5Request(groupId, topicName, partitions),
                _ => throw new Exception()
            };
        }

        public IListOffsetsRequest CreateListOffset(string topicName, int[] partitions)
        {
            return this.capabilities.GetVersionRange(ApiKey.ListOffsets) switch
            {
                { Min: <= 5, Max: >= 5 } => new ListOffsetsV5Request(-1, 0, topicName, partitions),
                _ => throw new Exception()
            };
        }
    }
}
