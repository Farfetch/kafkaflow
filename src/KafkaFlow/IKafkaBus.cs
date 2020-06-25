namespace KafkaFlow
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IKafkaBus
    {
        Task StartAsync(CancellationToken stopCancellationToken = default);

        Task StopAsync();
    }
}
