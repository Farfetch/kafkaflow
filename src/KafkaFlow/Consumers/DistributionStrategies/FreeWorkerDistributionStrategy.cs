namespace KafkaFlow.Consumers.DistributionStrategies
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;

    /// <summary>
    /// This strategy chooses the first free worker to process the message. When a worker finishes the processing, it notifies the worker pool that it is free to get a new message
    /// This is the fastest and resource-friendly strategy (the message buffer is not used) but messages with the same partition key can be delivered in different workers, so, no message order guarantee
    /// </summary>
    public class FreeWorkerDistributionStrategy : IDistributionStrategy
    {
        private readonly Channel<IWorker> freeWorkers = Channel.CreateUnbounded<IWorker>();

        /// <summary>
        /// Initializes the distribution strategy
        /// </summary>
        /// <param name="workers"></param>
        public void Init(IReadOnlyList<IWorker> workers)
        {
            foreach (var worker in workers)
            {
                worker.OnTaskCompleted(() => this.freeWorkers.Writer.WriteAsync(worker));
                this.freeWorkers.Writer.TryWrite(worker);
            }
        }

        /// <summary>
        /// Gets the next free worker regardless of the partition key informed 
        /// </summary>
        /// <param name="partitionKey">Message partition key</param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> used to cancel the operation.</param>
        /// <returns></returns>
        public Task<IWorker> GetWorkerAsync(byte[] partitionKey, CancellationToken cancellationToken)
        {
            return this.freeWorkers.Reader.ReadAsync(cancellationToken).AsTask();
        }
    }
}
