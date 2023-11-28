using System;

namespace KafkaFlow.Middlewares.ConsumerThrottling.Configuration
{
    /// <summary>
    /// An interface to configure the actions applied when a throttling threshold is met.
    /// </summary>
    public interface IConsumerThrottlingThresholdActionConfigurationBuilder
    {
        /// <summary>
        /// Configures the action to apply when the throttling threshold is met.
        /// The factory function takes an instance of <see cref="IDependencyResolver"/>, allowing you to resolve dependencies
        /// required to create an instance of <see cref="IConsumerThrottlingAction"/>.
        /// </summary>
        /// <param name="factory">
        /// A factory function that accepts an <see cref="IDependencyResolver"/> and returns an <see cref="IConsumerThrottlingAction"/>.
        /// </param>
        void Apply(Func<IDependencyResolver, IConsumerThrottlingAction> factory);
    }
}
