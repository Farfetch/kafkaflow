namespace KafkaFlow.Middlewares.ConsumerThrottling.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class ConsumerThrottlingConfigurationBuilder
        : IConsumerThrottlingMetricConfigurationBuilder,
            IConsumerThrottlingActionsConfigurationBuilder
    {
        private readonly List<Func<IDependencyResolver, IConsumerThrottlingMetric>> metrics = new();
        private readonly List<ConsumerThrottlingActionConfigurationBuilder> actions = new();
        private TimeSpan interval = TimeSpan.FromSeconds(5);

        public IConsumerThrottlingActionsConfigurationBuilder WithInterval(TimeSpan interval)
        {
            this.interval = interval;
            return this;
        }

        public IConsumerThrottlingMetricConfigurationBuilder AddMetric(Func<IDependencyResolver, IConsumerThrottlingMetric> factory)
        {
            this.metrics.Add(factory);
            return this;
        }

        public IConsumerThrottlingActionsConfigurationBuilder AddAction(Action<IConsumerThrottlingActionConfigurationBuilder> action)
        {
            var builder = new ConsumerThrottlingActionConfigurationBuilder();

            action(builder);

            this.actions.Add(builder);
            return this;
        }

        public ConsumerThrottlingConfiguration Build(IDependencyResolver resolver)
        {
            return new ConsumerThrottlingConfiguration(
                this.interval,
                this.metrics
                    .Select(x => x(resolver))
                    .ToList(),
                this.actions
                    .Select(x => new ConsumerThrottlingThreshold(x.Threshold, x.Factory(resolver)))
                    .ToList());
        }
    }
}
