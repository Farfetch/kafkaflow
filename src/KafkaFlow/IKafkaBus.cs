namespace KafkaFlow
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides access to the kafka bus operations
    /// </summary>
    public interface IKafkaBus
    {
        /// <summary>
        /// Starts all consumers 
        /// </summary>
        /// <param name="stopCancellationToken">A <see cref="T:System.Threading.CancellationToken" /> used to stop the operation.</param>
        /// <returns></returns>
        Task StartAsync(CancellationToken stopCancellationToken = default);

        /// <summary>
        /// Stops all consumers
        /// </summary>
        /// <returns></returns>
        Task StopAsync();
    }
}
