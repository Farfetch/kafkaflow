namespace KafkaFlow.Extensions.ProducerThrottling.Configuration
{
    using System.Collections.Generic;

    internal class ProducerThrottlingConfiguration
    {
        public IEnumerable<ProducerEvaluationConfiguration> Evaluations { get; }

        public ProducerThrottlingConfiguration(IEnumerable<ProducerEvaluationConfiguration> evaluations)
        {
            this.Evaluations = evaluations;
        }
    }
}
