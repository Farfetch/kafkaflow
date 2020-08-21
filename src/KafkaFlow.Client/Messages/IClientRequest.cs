namespace KafkaFlow.Client.Messages
{
    using KafkaFlow.Client.Protocol;

    internal interface IClientRequest<TResponse> : IClientRequest where TResponse : IClientResponse
    {
    }

    internal interface IClientRequest
    {
        ApiKey ApiKey { get; }
    }
}
