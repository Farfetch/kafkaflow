namespace KafkaFlow.Client
{
    using System;
    using System.Threading.Tasks;

    internal interface IKafkaCluster : IAsyncDisposable
    {
        IKafkaBroker AnyBroker { get; }

        ValueTask EnsureInitializationAsync();
    }
}
