namespace KafkaFlow.Client
{
    using System;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Messages;

    internal interface IKafkaHost : IDisposable
    {
        Task<TResponse> SendAsync<TResponse>(IClientRequest<TResponse> request)
            where TResponse : IClientResponse;
    }
}
