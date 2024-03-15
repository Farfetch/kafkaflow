using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KafkaFlow.Consumers.DistributionStrategies;

/// <summary>
/// This strategy applies a mod operator to the partition key and the total number of workers, the resulting number is the worker ID to be chosen
/// This algorithm is fast and creates a good work balance. Messages with the same partition key are always delivered in the same worker, so, message order is guaranteed
/// In cases where the number of partitions assigned to the consumer is small, this strategy can limit the number of available workers to distribute the messages.
/// </summary>
public class PartitionKeyDistributionStrategy : IWorkerDistributionStrategy
{
    private IReadOnlyList<IWorker> _workers;

    /// <inheritdoc />
    public void Initialize(IReadOnlyList<IWorker> workers)
    {
        _workers = workers;
    }

    /// <inheritdoc />
    public ValueTask<IWorker> GetWorkerAsync(WorkerDistributionContext context)
    {
        if (_workers.Count == 1)
        {
            return new ValueTask<IWorker>(_workers[0]);
        }

        return new ValueTask<IWorker>(
            context.ConsumerStoppedCancellationToken.IsCancellationRequested
                ? null
                : _workers.ElementAtOrDefault(context.Partition % _workers.Count));
    }
}
