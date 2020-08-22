namespace KafkaFlow.Client
{
    using System.Threading.Tasks;

    internal interface IKafkaCluster
    {
        IKafkaHost AnyHost { get; }

        IKafkaHost GetHost(int hostId);

        ValueTask EnsureInitializationAsync();

        ValueTask<IKafkaHost> GetCoordinatorAsync();
    }
}
