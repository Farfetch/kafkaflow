using System;
using System.Collections.Generic;
using KafkaFlow.Configuration;
using KafkaFlow.Consumers;
using KafkaFlow.Middlewares.ConsumerThrottling;
using KafkaFlow.Middlewares.ConsumerThrottling.Configuration;

namespace KafkaFlow;

/// <summary>
/// Provides extension methods for configuring consumer throttling in KafkaFlow.
/// </summary>
public static class ConsumerThrottlingConfigurationBuilderExtensions
{
    /// <summary>
    /// Configures consumer throttling using the specified configuration builder.
    /// </summary>
    /// <param name="middlewares">The consumer middleware configuration builder.</param>
    /// <param name="builder">An action to configure consumer throttling.</param>
    /// <returns>The original consumer middleware configuration builder with the throttling middleware added.</returns>
    public static IConsumerMiddlewareConfigurationBuilder Throttle(
        this IConsumerMiddlewareConfigurationBuilder middlewares,
        Action<IConsumerThrottlingMetricConfigurationBuilder> builder)
    {
        var instance = new ConsumerThrottlingConfigurationBuilder();

        builder(instance);

        return middlewares.Add(resolver => new ConsumerThrottlingMiddleware(instance.Build(resolver)));
    }

    /// <summary>
    /// Configures throttling based on the lag of specified consumers.
    /// </summary>
    /// <param name="builder">The consumer throttling metric configuration builder.</param>
    /// <param name="consumerNames">The names of the other consumers to monitor for lag.</param>
    /// <returns>The original consumer throttling metric configuration builder with the lag metric added.</returns>
    public static IConsumerThrottlingMetricConfigurationBuilder OnConsumerLag(
        this IConsumerThrottlingMetricConfigurationBuilder builder,
        IEnumerable<string> consumerNames)
    {
        builder.AddMetric(
            resolver => new ConsumerThrottlingKafkaLagMetric(
                resolver.Resolve<IConsumerAccessor>(),
                consumerNames));

        return builder;
    }

    /// <summary>
    /// Configures throttling based on the lag of specified consumers.
    /// </summary>
    /// <param name="builder">The consumer throttling metric configuration builder.</param>
    /// <param name="consumerNames">The names of the other consumers to monitor for lag.</param>
    /// <returns>The original consumer throttling metric configuration builder with the lag metric added.</returns>
    public static IConsumerThrottlingMetricConfigurationBuilder OnConsumerLag(
        this IConsumerThrottlingMetricConfigurationBuilder builder,
        params string[] consumerNames)
    {
        builder.AddMetric(
            resolver => new ConsumerThrottlingKafkaLagMetric(
                resolver.Resolve<IConsumerAccessor>(),
                consumerNames));

        return builder;
    }

    /// <summary>
    /// Configures an action to apply a delay when the throttling threshold is met.
    /// </summary>
    /// <param name="builder">The consumer throttling threshold action configuration builder.</param>
    /// <param name="delay">The delay to apply.</param>
    public static void ApplyDelay(this IConsumerThrottlingThresholdActionConfigurationBuilder builder, TimeSpan delay)
    {
        builder.Apply(_ => new ConsumerThrottlingDelayAction(delay));
    }

    /// <summary>
    /// Configures an action to apply a delay in milliseconds when the throttling threshold is met.
    /// </summary>
    /// <param name="builder">The consumer throttling threshold action configuration builder.</param>
    /// <param name="delayMs">The delay in milliseconds to apply.</param>
    public static void ApplyDelay(this IConsumerThrottlingThresholdActionConfigurationBuilder builder, int delayMs)
    {
        builder.Apply(_ => new ConsumerThrottlingDelayAction(TimeSpan.FromMilliseconds(delayMs)));
    }
}
