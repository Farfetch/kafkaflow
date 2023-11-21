using System;

namespace KafkaFlow.Middlewares.ConsumerThrottling.Configuration
{
    internal class ConsumerThrottlingActionConfigurationBuilder
        : IConsumerThrottlingActionConfigurationBuilder,
            IConsumerThrottlingThresholdActionConfigurationBuilder
    {
        public long Threshold { get; private set; }

        public Func<IDependencyResolver, IConsumerThrottlingAction> Factory { get; private set; }

        public IConsumerThrottlingThresholdActionConfigurationBuilder AboveThreshold(long threshold)
        {
            this.Threshold = threshold;
            return this;
        }

        public void Apply(Func<IDependencyResolver, IConsumerThrottlingAction> factory)
        {
            this.Factory = factory;
        }
    }
}
