namespace KafkaFlow.Client.Protocol.Messages
{
    using KafkaFlow.Client.Protocol;

    public interface IBrokerCapabilities
    {
        ApiVersionRange GetVersionRange(ApiKey api);
    }
}
