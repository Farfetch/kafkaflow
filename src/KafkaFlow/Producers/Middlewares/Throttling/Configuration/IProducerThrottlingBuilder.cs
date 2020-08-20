namespace KafkaFlow.Producers.Middlewares.Throttling.Configuration
{
    using System;
    using KafkaFlow.Producers.Middlewares.Throttling.Evaluations;

    /// <summary>
    /// No needed
    /// </summary>
    public interface IProducerThrottlingBuilder
    {
        /// <summary>
        /// Adds an evaluation to the throttling
        /// </summary>
        /// <param name="interval">The time interval between evaluation executions </param>
        /// <param name="factory">Factory of <see cref="IEvaluation"/></param>
        /// <returns></returns>
        IProducerEvaluationBuilder AddEvaluation(TimeSpan interval, Factory<IEvaluation> factory);
    }
}
