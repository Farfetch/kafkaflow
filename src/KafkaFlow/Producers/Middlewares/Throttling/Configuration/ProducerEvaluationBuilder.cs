namespace KafkaFlow.Producers.Middlewares.Throttling.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using KafkaFlow.Producers.Middlewares.Throttling.Actions;
    using KafkaFlow.Producers.Middlewares.Throttling.Evaluations;

    internal class ProducerEvaluationBuilder : IProducerEvaluationBuilder
    {
        private readonly List<Factory<IAction>> actions = new();
        private readonly TimeSpan interval;
        private readonly Factory<IEvaluation> factory;

        public ProducerEvaluationBuilder(Factory<IEvaluation> factory, TimeSpan interval)
        {
            this.interval = interval;
            this.factory = factory;
        }

        public IProducerEvaluationBuilder AddAction(Factory<IAction> action)
        {
            this.actions.Add(action);
            return this;
        }

        public ProducerEvaluationConfiguration Build(IDependencyResolver resolver)
        {
            return new ProducerEvaluationConfiguration(
                this.factory(resolver),
                this.interval,
                this.actions
                    .Select(factory => factory(resolver))
                    .ToList());
        }
    }
}
