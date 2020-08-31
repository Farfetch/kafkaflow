namespace KafkaFlow.Client.Protocol.Messages
{
    using KafkaFlow.Client.Protocol;

    public interface IRequestFactory
    {
        IProduceRequest CreateProduce(ProduceAcks acks, int timeout);

        IMetadataRequest CreateMetadata();
    }
}
