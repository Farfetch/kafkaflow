namespace KafkaFlow.Client
{
    using System;
    using KafkaFlow.Client.Protocol;
    using KafkaFlow.Client.Protocol.Messages;

    internal interface IKafkaBroker : IDisposable
    {
        IRequestFactory RequestFactory { get; }

        IBrokerConnection Connection { get; }
    }
}
