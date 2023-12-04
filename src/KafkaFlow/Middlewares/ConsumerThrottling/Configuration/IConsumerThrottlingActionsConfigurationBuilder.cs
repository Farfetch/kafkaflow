using System;

namespace KafkaFlow.Middlewares.ConsumerThrottling.Configuration;

/// <summary>
/// Provides methods to configure throttling actions for KafkaFlow consumers.
/// </summary>
public interface IConsumerThrottlingActionsConfigurationBuilder
{
    /// <summary>
    /// Adds a throttling action to the configuration.
    /// </summary>
    /// <param name="action">An action that configures the throttling action.</param>
    /// <returns>The same instance of the <see cref="IConsumerThrottlingActionsConfigurationBuilder"/> for method chaining.</returns>
    IConsumerThrottlingActionsConfigurationBuilder AddAction(Action<IConsumerThrottlingActionConfigurationBuilder> action);
}
