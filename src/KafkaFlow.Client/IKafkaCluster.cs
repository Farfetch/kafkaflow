namespace KafkaFlow.Client
{
    using System.Threading.Tasks;

    public interface IKafkaCluster
    {
        IKafkaBroker AnyBroker { get; }

        IKafkaBroker GetBroker(int hostId);

        ValueTask EnsureInitializationAsync();

        ValueTask<IKafkaBroker> GetCoordinatorAsync();
    }
}
