namespace KafkaFlow.Client.Protocol
{
    using System;
    using System.Threading.Tasks;

    public interface IKafkaHostConnection : IDisposable
    {
        Task<TResponse> SendAsync<TResponse>(IRequestMessage<TResponse> request)
            where TResponse : IResponse, new();
    }
}
