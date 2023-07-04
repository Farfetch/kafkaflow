namespace KafkaFlow.Consumers.DistributionStrategies
{
    using System.Collections.Generic;
    using System.Linq;
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

        /// <inheritdoc />
        public void Init(IReadOnlyList<IWorker> workers)
        {
            this.workers = workers;
        }

        /// <inheritdoc />
        public Task<IWorker> GetWorkerAsync(byte[] partitionKey, CancellationToken cancellationToken)
        {
            if (partitionKey is null || this.workers.Count == 1)
            {
                return Task.FromResult(this.workers[0]);
            }

            var bytesSum = 0;

            for (int i = 0; i < partitionKey.Length; i++)
            {
                bytesSum += partitionKey[i];
            }

            return Task.FromResult(
                cancellationToken.IsCancellationRequested
                    ? null
                    : this.workers.ElementAtOrDefault(bytesSum % this.workers.Count));
        }
    }
}
