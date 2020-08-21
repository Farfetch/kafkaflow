namespace KafkaFlow.Client.Messages.Adapters
{
    using System;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Messages;

    internal interface IHostConnectionProxy : IDisposable
    {
        Task<TResponse> SendAsync<TResponse>(IClientRequest<TResponse> request)
            where TResponse : IClientResponse;
    }
}
