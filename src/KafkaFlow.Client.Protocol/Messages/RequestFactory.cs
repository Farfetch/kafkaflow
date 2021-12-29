namespace KafkaFlow.Client.Protocol.Messages
{
    using KafkaFlow.Client.Protocol.Messages.Implementations;
    using KafkaFlow.Client.Protocol.Messages.Implementations.ApiVersion;
    using KafkaFlow.Client.Protocol.Messages.Implementations.ListOffsets;
    using KafkaFlow.Client.Protocol.Messages.Implementations.Metadata;
    using KafkaFlow.Client.Protocol.Messages.Implementations.OffsetFetch;
    using KafkaFlow.Client.Protocol.Messages.Implementations.Produce;

    public class RequestFactory : IRequestFactory
    {
        private IBrokerCapabilities capabilities;

        public void SetBrokerCapabilities(BrokerCapabilities capabilities)
        {
            this.capabilities = capabilities;
        }

        public IApiVersionRequest CreateApiVersion() => new ApiVersionV2Request();

        public IProduceRequest CreateProduce(ProduceAcks acks, int timeout)
        {
            // var cap = this.capabilities.GetVersionRange(ApiKey.Produce);

            return new ProduceV8Request(acks, timeout);
        }

        public IMetadataRequest CreateMetadata()
        {
            // var cap = this.capabilities.GetVersionRange(ApiKey.Produce);

            return new MetadataV9Request();
        }

        public IOffsetFetchRequest CreateOffsetFetch(string groupId, string topicName, int[] partitions)
        {
            // var cap = this.capabilities.GetVersionRange(ApiKey.Produce);

            return new OffsetFetchV5Request(groupId, topicName, partitions);
        }

        public IListOffsetsRequest CreateListOffset(string topicName, int[] partitions)
        {
            // var cap = this.capabilities.GetVersionRange(ApiKey.Produce);

            return new ListOffsetsV5Request(-1, 0, topicName, partitions);
        }
    }
}
