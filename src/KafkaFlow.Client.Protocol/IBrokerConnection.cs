namespace KafkaFlow.Client.Protocol
{
    using System;
    using System.Threading.Tasks;

    public interface IBrokerConnection : IAsyncDisposable
    {
        Task<TResponse> SendAsync<TResponse>(IRequestMessage<TResponse> request)
            where TResponse : class, IResponse;

        BrokerAddress Address { get; }
    }
}
