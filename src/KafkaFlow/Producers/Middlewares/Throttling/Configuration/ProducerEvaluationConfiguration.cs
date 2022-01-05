namespace KafkaFlow.Producers.Middlewares.Throttling.Configuration
{
    using System;
    using System.Collections.Generic;
    using KafkaFlow.Producers.Middlewares.Throttling.Actions;
    using KafkaFlow.Producers.Middlewares.Throttling.Evaluations;

    internal class ProducerEvaluationConfiguration
    {
        public ProducerEvaluationConfiguration(
            IEvaluation evaluation,
            TimeSpan interval,
            IReadOnlyList<IAction> actions)
        {
            this.Evaluation = evaluation;
            this.Interval = interval;
            this.Actions = actions;
        }

        public IEvaluation Evaluation { get; }

        public TimeSpan Interval { get; }

        public IReadOnlyList<IAction> Actions { get; }
    }
}
