namespace KafkaFlow.Client.Metrics
{
    using System.Threading.Tasks;

    public interface IMetricReader
    {
        Task<long> GetLagAsync(
            string topic,
            string consumerGroup);
    }
}
