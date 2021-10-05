namespace KafkaFlow.Extensions.ProducerThrottling.Configuration
{
    using System;
    using KafkaFlow.Extensions.ProducerThrottling.Evaluations;

    /// <summary>
    /// No needed
    /// </summary>
    public interface IProducerThrottlingBuilder
    {
        /// <summary>
        /// Adds an evaluation to the throttling
        /// </summary>
        /// <param name="factory">Factory of <see cref="IEvaluation"/></param>
        /// <param name="interval">The time interval between evaluation executions </param>
        /// <returns></returns>
        IProducerEvaluationBuilder AddEvaluation(Factory<IEvaluation> factory, TimeSpan? interval = null);
    }
}
