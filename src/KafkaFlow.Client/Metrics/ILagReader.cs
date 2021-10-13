namespace KafkaFlow.Client.Metrics
{
    using System.Threading.Tasks;

    public interface ILagReader
    {
        Task<long> GetLagAsync(
            string topic,
            string consumerGroup);
    }
}
