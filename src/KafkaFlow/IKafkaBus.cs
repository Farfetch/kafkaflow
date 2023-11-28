using System.Threading;
using System.Threading.Tasks;
using KafkaFlow.Consumers;
using KafkaFlow.Producers;

namespace KafkaFlow
{
    /// <summary>
    /// Provides access to the kafka bus operations
    /// </summary>
    public interface IKafkaBus
    {
        /// <summary>
        /// Gets all configured consumers
        /// </summary>
        IConsumerAccessor Consumers { get; }

        /// <summary>
        /// Gets all configured producers
        /// </summary>
        IProducerAccessor Producers { get; }

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
