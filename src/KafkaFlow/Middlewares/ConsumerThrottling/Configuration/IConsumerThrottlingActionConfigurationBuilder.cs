namespace KafkaFlow.Middlewares.ConsumerThrottling.Configuration;

/// <summary>
/// Provides a builder interface for setting up actions in the KafkaFlow consumer throttling feature.
/// </summary>
public interface IConsumerThrottlingActionConfigurationBuilder
{
    /// <summary>
    /// Defines the threshold at which the throttling action should be triggered.
    /// </summary>
    /// <param name="threshold">The threshold value for the action.</param>
    /// <returns>An instance of <see cref="IConsumerThrottlingThresholdActionConfigurationBuilder"/> to further configure the action.</returns>
    IConsumerThrottlingThresholdActionConfigurationBuilder AboveThreshold(long threshold);
}
