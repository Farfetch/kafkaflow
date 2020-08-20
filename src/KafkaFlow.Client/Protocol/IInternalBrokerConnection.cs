namespace KafkaFlow.Client.Protocol
{
    using System.Threading.Tasks;

    internal interface IInternalBrokerConnection
    {
        BrokerAddress Address { get; }

        Task<TResponse> SendAsync<TResponse>(IRequestMessage<TResponse> request)
            where TResponse : class, IResponse;
    }
}
