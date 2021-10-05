namespace KafkaFlow.Extensions.ProducerThrottling.Configuration
{
    using System;
    using System.Collections.Generic;
    using KafkaFlow.Extensions.ProducerThrottling.Actions;
    using KafkaFlow.Extensions.ProducerThrottling.Evaluations;

    internal class ProducerEvaluationConfiguration
    {
        public IEvaluation Evaluation { get; }

        public TimeSpan Frequency { get; }

        public IEnumerable<(long threshold, IAction action)> Actions { get; }

        public ProducerEvaluationConfiguration(
            IEvaluation evaluation,
            TimeSpan frequency,
            IEnumerable<(long threshold, IAction action)> actions)
        {
            this.Evaluation = evaluation;
            this.Frequency = frequency;
            this.Actions = actions;
        }
    }
}
