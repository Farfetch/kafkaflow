namespace KafkaFlow.Client
{
    using System;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Protocol;
    using KafkaFlow.Client.Protocol.Messages;

    internal interface IKafkaHost : IDisposable
    {
        IRequestFactory RequestFactory { get; }

        Task<TResponse> SendAsync<TResponse>(IRequestMessage<TResponse> request)
            where TResponse : class, IResponse;
    }
}
