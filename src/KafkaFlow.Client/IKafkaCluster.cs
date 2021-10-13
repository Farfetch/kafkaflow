namespace KafkaFlow.Client
{
    using System.Threading.Tasks;

    public interface IKafkaCluster
    {
        IKafkaBroker AnyBroker { get; }

        ValueTask EnsureInitializationAsync();
    }
}
