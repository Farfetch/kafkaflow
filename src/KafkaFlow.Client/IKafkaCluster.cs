namespace KafkaFlow.Client
{
    using System;
    using System.Threading.Tasks;

    public interface IKafkaCluster : IAsyncDisposable
    {
        IKafkaBroker AnyBroker { get; }

        IKafkaBroker GetBroker(int hostId);

        ValueTask EnsureInitializationAsync();
    }
}
