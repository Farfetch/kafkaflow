namespace KafkaFlow.Consumers.DistributionStrategies
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// This strategy sums all bytes in the partition key and apply a mod operator with the total number of workers, the resulting number is the worker ID to be chosen
    /// This algorithm is fast and creates a good work balance. Messages with the same partition key are always delivered in the same worker, so, message order is guaranteed
    /// Set an optimal message buffer value to avoid idle workers (it will depends how many messages with the same partition key are consumed)
    /// </summary>
    public class BytesSumDistributionStrategy : IDistributionStrategy
    {
        private IReadOnlyList<IWorker> workers;

        /// <summary>
        /// Initializes the distribution strategy
        /// </summary>
        /// <param name="workers"></param>
        public void Init(IReadOnlyList<IWorker> workers)
        {
            this.workers = workers;
        }

        /// <summary>
        /// Gets the worker based on the partition key informed 
        /// </summary>
        /// <param name="partitionKey">Message partition key</param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> used to cancel the operation.</param>
        /// <returns></returns>
        public Task<IWorker> GetWorkerAsync(byte[] partitionKey, CancellationToken cancellationToken)
        {
            return cancellationToken.IsCancellationRequested ? 
                null : 
                Task.FromResult(this.workers.ElementAtOrDefault(partitionKey.Sum(x => x) % this.workers.Count));
        }
    }
}
