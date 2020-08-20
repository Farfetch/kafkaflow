namespace KafkaFlow.Client.Protocol.Messages
{
    using System;
    using KafkaFlow.Client.Protocol.Messages.Implementations.ListOffsets;
    using KafkaFlow.Client.Protocol.Messages.Implementations.Metadata;
    using KafkaFlow.Client.Protocol.Messages.Implementations.OffsetFetch;

    /// <summary>
    /// Factory class used to create message requests
    /// </summary>
    public class RequestFactory : IRequestFactory
    {
        private readonly IBrokerCapabilities capabilities;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestFactory"/> class.
        /// </summary>
        /// <param name="capabilities">The broker capabilities instance</param>
        public RequestFactory(IBrokerCapabilities capabilities)
        {
            this.capabilities = capabilities;
        }

        /// <inheritdoc/>
        public IMetadataRequest CreateMetadata(params string[] topics)
        {
            return this.capabilities.GetVersionRange(ApiKey.Metadata) switch
            {
                { Min: <= 9, Max: >= 9 } => new MetadataV9Request(topics),
                _ => throw new Exception()
            };
        }

        /// <inheritdoc/>
        public IOffsetFetchRequest CreateOffsetFetch(string groupId, string topicName, int[] partitions)
        {
            return this.capabilities.GetVersionRange(ApiKey.OffsetFetch) switch
            {
                { Min: <= 5, Max: >= 5 } => new OffsetFetchV5Request(groupId, topicName, partitions),
                _ => throw new Exception()
            };
        }

        /// <inheritdoc/>
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
