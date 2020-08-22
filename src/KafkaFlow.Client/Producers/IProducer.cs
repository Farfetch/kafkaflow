namespace KafkaFlow.Client.Producers
{
    using System.Threading.Tasks;

    public interface IProducer
    {
        Task<ProduceResult> ProduceAsync(ProduceData data);
    }
}
