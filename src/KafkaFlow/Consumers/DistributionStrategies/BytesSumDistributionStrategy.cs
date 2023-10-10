namespace KafkaFlow.Consumers.DistributionStrategies;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// This strategy sums all bytes in the partition key and apply a mod operator with the total number of workers, the resulting number is the worker ID to be chosen
/// This algorithm is fast and creates a good work balance. Messages with the same partition key are always delivered in the same worker, so, message order is guaranteed
/// Set an optimal message buffer value to avoid idle workers (it will depends how many messages with the same partition key are consumed)
/// </summary>
public class BytesSumDistributionStrategy : IWorkerDistributionStrategy
{
    private IReadOnlyList<IWorker> workers;

    /// <inheritdoc />
    public void Initialize(IReadOnlyList<IWorker> workers)
    {
        this.workers = workers;
    }

    /// <inheritdoc />
    public ValueTask<IWorker> GetWorkerAsync(WorkerDistributionContext context)
    {
        if (context.RawMessageKey is null || this.workers.Count == 1)
        {
            return new ValueTask<IWorker>(this.workers[0]);
        }

        var bytesSum = 0;

        for (var i = 0; i < context.RawMessageKey.Value.Length; i++)
        {
            bytesSum += context.RawMessageKey.Value.Span[i];
        }

        return new ValueTask<IWorker>(
            context.ConsumerStoppedCancellationToken.IsCancellationRequested
                ? null
                : this.workers.ElementAtOrDefault(bytesSum % this.workers.Count));
    }
}
