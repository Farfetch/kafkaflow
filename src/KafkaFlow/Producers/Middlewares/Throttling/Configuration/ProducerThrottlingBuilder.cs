namespace KafkaFlow.Producers.Middlewares.Throttling.Configuration
{
    using System;
    using System.Collections.Generic;
    using KafkaFlow.Producers.Middlewares.Throttling.Evaluations;

    internal class ProducerThrottlingBuilder : IProducerThrottlingBuilder
    {
        private readonly List<ProducerEvaluationBuilder> evaluationBuilders = new();

        public IReadOnlyList<ProducerEvaluationBuilder> EvaluationBuilders => this.evaluationBuilders.AsReadOnly();

        public IProducerEvaluationBuilder AddEvaluation(TimeSpan interval, Factory<IEvaluation> factory)
        {
            var evaluationBuilder = new ProducerEvaluationBuilder(factory, interval);

            this.evaluationBuilders.Add(evaluationBuilder);

            return evaluationBuilder;
        }
    }
}
