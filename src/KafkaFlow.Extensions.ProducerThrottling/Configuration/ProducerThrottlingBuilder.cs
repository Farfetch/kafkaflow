namespace KafkaFlow.Extensions.ProducerThrottling.Configuration
{
    using System;
    using System.Collections.Generic;
    using KafkaFlow.Extensions.ProducerThrottling.Evaluations;

    internal class ProducerThrottlingBuilder : IProducerThrottlingBuilder
    {
        public List<ProducerEvaluationBuilder> EvaluationBuilders { get; }

        public ProducerThrottlingBuilder()
        {
            this.EvaluationBuilders = new List<ProducerEvaluationBuilder>();
        }

        public IProducerEvaluationBuilder AddEvaluation(Factory<IEvaluation> factory, TimeSpan? interval = null)
        {
            var evaluationBuilder = new ProducerEvaluationBuilder(factory, interval);

            this.EvaluationBuilders.Add(evaluationBuilder);

            return evaluationBuilder;
        }
    }
}
