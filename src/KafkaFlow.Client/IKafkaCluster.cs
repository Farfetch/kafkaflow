namespace KafkaFlow.Client
{
    using System.Threading.Tasks;

    internal interface IKafkaCluster
    {
        IKafkaHost[] Hosts { get; }

        IKafkaHost AnyHost { get; }

        IKafkaHost GetHost(int hostId);

        ValueTask<IKafkaHost> GetCoordinatorAsync();
    }
}