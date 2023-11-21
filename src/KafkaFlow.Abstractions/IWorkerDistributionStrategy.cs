using System.Collections.Generic;
using System.Threading.Tasks;

namespace KafkaFlow;

/// <summary>
/// An interface used to create a distribution strategy
/// </summary>
public interface IWorkerDistributionStrategy
{
    /// <summary>
    /// Initializes the distribution strategy, this method is called when a consumer is started
    /// </summary>
    /// <param name="workers">List of workers to be initialized</param>
    void Initialize(IReadOnlyList<IWorker> workers);

    /// <summary>
    /// Retrieves an available worker based on the provided distribution strategy context.
    /// </summary>
    /// <param name="context">The distribution strategy context containing message and consumer details.</param>
    /// <returns>The selected <see cref="IWorker"/> instance.</returns>
    ValueTask<IWorker> GetWorkerAsync(WorkerDistributionContext context);
}
