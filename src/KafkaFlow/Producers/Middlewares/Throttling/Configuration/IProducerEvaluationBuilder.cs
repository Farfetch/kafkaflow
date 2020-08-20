namespace KafkaFlow.Producers.Middlewares.Throttling.Configuration
{
    using KafkaFlow.Producers.Middlewares.Throttling.Actions;

    /// <summary>
    /// No needed
    /// </summary>
    public interface IProducerEvaluationBuilder
    {
        /// <summary>
        /// Adds an action to the evaluation
        /// </summary>
        /// <param name="threshold">The threshold value</param>
        /// <param name="evaluation">Factory of <see cref="IAction"/></param>
        /// <returns></returns>
        IProducerEvaluationBuilder AddAction(Factory<IAction> evaluation);
    }
}
