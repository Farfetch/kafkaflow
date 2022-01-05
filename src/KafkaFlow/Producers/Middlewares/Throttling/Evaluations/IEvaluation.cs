namespace KafkaFlow.Producers.Middlewares.Throttling.Evaluations
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using KafkaFlow.Producers.Middlewares.Throttling.Actions;

    /// <summary>
    /// An interface used to create threshold evaluations
    /// </summary>
    public interface IEvaluation
    {
        /// <summary>
        /// The evaluation function
        /// </summary>
        /// <param name="context"></param>
        /// <param name="actions"></param>
        /// <returns></returns>
        Task<IAction> EvaluateAsync(IMessageContext context, IReadOnlyList<IAction> actions);
    }
}
