namespace KafkaFlow.Producers.Middlewares.Throttling.Evaluations
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using KafkaFlow.Configuration;
    using KafkaFlow.Producers.Middlewares.Throttling.Actions;

    /// <summary>
    /// An interface used to create threshold evaluations
    /// </summary>
    public interface IEvaluation
    {
        /// <summary>
        /// The evaluation function
        /// </summary>
        /// <param name="producerConfiguration">The producer configuration</param>
        /// <param name="actions">The configured actions</param>
        /// <param name="cancellationToken">A cancellation token that is triggered when the bus is stopping</param>
        /// <returns></returns>
        Task<IAction> EvaluateAsync(
            IProducerConfiguration producerConfiguration,
            IReadOnlyList<IAction> actions,
            CancellationToken cancellationToken);
    }
}
