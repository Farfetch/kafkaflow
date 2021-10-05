namespace KafkaFlow.Extensions.ProducerThrottling.Configuration
{
    using KafkaFlow.Extensions.ProducerThrottling.Actions;

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
        IProducerEvaluationBuilder AddAction(long threshold, Factory<IAction> evaluation);
    }
}
