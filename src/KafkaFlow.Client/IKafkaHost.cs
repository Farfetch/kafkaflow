namespace KafkaFlow.Client
{
    using System;
    using KafkaFlow.Client.Protocol;
    using KafkaFlow.Client.Protocol.Messages;

    internal interface IKafkaHost : IDisposable
    {
        IRequestFactory RequestFactory { get; }

        IKafkaHostConnection Connection { get; }
    }
}
