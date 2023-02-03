namespace KafkaFlow.Middlewares.ConsumerThrottling.Configuration
{
    using System;

    /// <summary>
    /// An interface to configure the throttling metrics for KafkaFlow consumers.
    /// </summary>
    public interface IConsumerThrottlingMetricConfigurationBuilder
    {
        /// <summary>
        /// Configures the metric used for throttling the consumer.
        /// The factory function takes an instance of <see cref="IDependencyResolver"/>, allowing you to resolve dependencies
        /// required to create an instance of <see cref="IConsumerThrottlingMetric"/>.
        /// </summary>
        /// <param name="factory">
        /// A factory function that accepts an <see cref="IDependencyResolver"/> and returns an <see cref="IConsumerThrottlingMetric"/>.
        /// </param>
        /// <returns>The <see cref="IConsumerThrottlingMetricConfigurationBuilder"/> for method chaining.</returns>
        IConsumerThrottlingMetricConfigurationBuilder AddMetric(Func<IDependencyResolver, IConsumerThrottlingMetric> factory);

        /// <summary>
        /// Sets the interval at which the throttling metric is checked.
        /// The throttling actions will be applied based on this interval.
        /// </summary>
        /// <param name="interval">The interval for checking the metric.</param>
        /// <returns>The <see cref="IConsumerThrottlingActionsConfigurationBuilder"/> to continue configuring throttling actions.</returns>
        IConsumerThrottlingActionsConfigurationBuilder WithInterval(TimeSpan interval);
    }
}
