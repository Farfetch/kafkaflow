namespace KafkaFlow.Client.Protocol.Messages
{
    using System;
    using KafkaFlow.Client.Protocol.Messages.Implementations.ApiVersion;
    using KafkaFlow.Client.Protocol.Messages.Implementations.ListOffsets;
    using KafkaFlow.Client.Protocol.Messages.Implementations.Metadata;
    using KafkaFlow.Client.Protocol.Messages.Implementations.OffsetFetch;
    using KafkaFlow.Client.Protocol.Messages.Implementations.Produce;

    /// <summary>
    /// Factory class used to create message requests
    /// </summary>
    public class RequestFactory : IRequestFactory
    {
        private IBrokerCapabilities? capabilities;

        /// <summary>
        /// Sets the broker capablities
        /// </summary>
        /// <param name="capabilities"></param>
        public void SetBrokerCapabilities(BrokerCapabilities capabilities)
        {
            this.capabilities = capabilities;
        }

        /// <summary>
        /// Creates a ApiVersion request
        /// </summary>
        /// <returns></returns>
        public IApiVersionRequest CreateApiVersion() => new ApiVersionV2Request();

        /// <inheritdoc/>
        public IMetadataRequest CreateMetadata()
        {
            return this.capabilities.GetVersionRange(ApiKey.Metadata) switch
            {
                { Min: <= 9, Max: >= 9 } => new MetadataV9Request(),
                _ => throw new Exception()
            };
        }

        /// <inheritdoc/>
        public IOffsetFetchRequest CreateOffsetFetch(string groupId)
        {
            return this.capabilities.GetVersionRange(ApiKey.OffsetFetch) switch
            {
                { Min: <= 5, Max: >= 5 } => new OffsetFetchV5Request(groupId),
                _ => throw new Exception()
            };
        }

        /// <inheritdoc/>
        public IListOffsetsRequest CreateListOffset()
        {
            return this.capabilities.GetVersionRange(ApiKey.ListOffsets) switch
            {
                { Min: <= 5, Max: >= 5 } => new ListOffsetsV5Request(),
                _ => throw new Exception()
            };
        }

        /// <inheritdoc/>
        public IProduceRequest CreateProduce(ProduceAcks acks, int timeout)
        {
            return this.capabilities.GetVersionRange(ApiKey.ListOffsets) switch
            {
                { Min: <= 8, Max: >= 8 } => new ProduceV8Request(acks, timeout),
                _ => throw new Exception()
            };
        }
    }
}
