namespace KafkaFlow.Client
{
    using System;
    using KafkaFlow.Client.Protocol;
    using KafkaFlow.Client.Protocol.Messages;

    public interface IKafkaBroker : IAsyncDisposable
    {
        IBrokerConnection Connection { get; }

        IRequestFactory RequestFactory { get; }
    }
}
