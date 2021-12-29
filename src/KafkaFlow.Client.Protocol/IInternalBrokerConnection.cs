namespace KafkaFlow.Client.Protocol
{
    using System.Threading.Tasks;

    internal interface IInternalBrokerConnection
    {
        BrokerAddress Address { get; }

        Task<TResponse> InternalSendAsync<TResponse>(IRequestMessage<TResponse> request)
            where TResponse : class, IResponse;
    }
}
