namespace KafkaFlow.Client.Messages.Adapters
{
    using KafkaFlow.Client.Protocol;

    internal interface IHostCapabilities
    {
        ApiVersionRange GetVersionRange(ApiKey api);
    }
}