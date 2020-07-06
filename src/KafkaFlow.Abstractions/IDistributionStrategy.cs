namespace KafkaFlow
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the interface to be implemented by custom distribution strategies
    /// </summary>
    public interface IDistributionStrategy
    {
        /// <summary>
        /// Initializes the distribution strategy and its workers
        /// </summary>
        /// <param name="workers">List of workers to be initialized</param>
        void Init(IReadOnlyList<IWorker> workers);

        /// <summary>
        /// Gets a available worker based on the distribution strategy implemented 
        /// </summary>
        /// <param name="partitionKey">Message partition key</param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> used to cancel the operation</param>
        /// <returns></returns>
        Task<IWorker> GetWorkerAsync(byte[] partitionKey, CancellationToken cancellationToken);
    }
}
