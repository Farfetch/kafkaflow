namespace KafkaFlow.Client.Protocol.Messages
{
    using KafkaFlow.Client.Protocol;
    using KafkaFlow.Client.Protocol.Messages.Implementations;

    public class RequestFactory : IRequestFactory
    {
        private readonly IHostCapabilities capabilities;

        public RequestFactory(IHostCapabilities capabilities)
        {
            this.capabilities = capabilities;
        }

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
    }
}
