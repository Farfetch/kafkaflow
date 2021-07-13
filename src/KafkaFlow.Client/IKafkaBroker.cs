namespace KafkaFlow.Client
{
    using System;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Protocol;
    using KafkaFlow.Client.Protocol.Messages;

    public interface IKafkaBroker : IAsyncDisposable
    {
        IBrokerConnection Connection { get; }

        BrokerAddress Address { get; }

        int NodeId { get; }

        Task<IRequestFactory> GetRequestFactoryAsync();
    }
}
