namespace KafkaFlow.Client.Protocol.Messages
{
    using KafkaFlow.Client.Protocol;

    public interface IHostCapabilities
    {
        ApiVersionRange GetVersionRange(ApiKey api);
    }
}