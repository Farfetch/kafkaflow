namespace KafkaFlow.Client
{
    using System.Threading.Tasks;

    internal interface IKafkaCluster
    {
        IKafkaBroker AnyBroker { get; }

        IKafkaBroker GetBroker(int hostId);

        ValueTask EnsureInitializationAsync();

        ValueTask<IKafkaBroker> GetCoordinatorAsync();
    }
}
