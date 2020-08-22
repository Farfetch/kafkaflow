namespace KafkaFlow.Client.Messages.Adapters
{
    using System.Threading.Tasks;
    using KafkaFlow.Client.Messages;

    internal interface IHostConnectionAdapter<in TRequest, TResponse>
        where TResponse : IClientResponse
        where TRequest : IClientRequest<TResponse>
    {
        Task<TResponse> SendAsync(TRequest request);
    }
}
