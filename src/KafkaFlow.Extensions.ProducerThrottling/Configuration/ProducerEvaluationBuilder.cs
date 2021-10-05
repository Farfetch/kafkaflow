namespace KafkaFlow.Extensions.ProducerThrottling.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using KafkaFlow.Extensions.ProducerThrottling.Actions;
    using KafkaFlow.Extensions.ProducerThrottling.Evaluations;

    internal class ProducerEvaluationBuilder : IProducerEvaluationBuilder
    {
        private TimeSpan Interval { get; }

        private Factory<IEvaluation> factory { get; }

        private IList<(long threshold, Factory<IAction> action)> Actions { get; }

        public ProducerEvaluationBuilder(Factory<IEvaluation> factory, TimeSpan? interval)
        {
            this.Interval = interval ?? TimeSpan.FromSeconds(1);
            this.factory = factory;
            this.Actions = new List<(long threshold, Factory<IAction> action)>();
        }

        public IProducerEvaluationBuilder AddAction(long threshold, Factory<IAction> action)
        {
            this.Actions.Add((threshold, action));

            return this;
        }

        public ProducerEvaluationConfiguration Build(IDependencyResolver resolver)
        {
            return new ProducerEvaluationConfiguration(
                this.factory(resolver),
                this.Interval,
                this.Actions.Select(a => (a.threshold, a.action(resolver))).ToList());
        }
    }
}
