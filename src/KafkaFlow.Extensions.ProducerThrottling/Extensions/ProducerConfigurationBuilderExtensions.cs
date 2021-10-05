namespace KafkaFlow.Extensions.ProducerThrottling.Extensions
{
    using System;
    using KafkaFlow.Configuration;
    using KafkaFlow.Extensions.ProducerThrottling.Configuration;

    /// <summary>
    /// No needed
    /// </summary>
    public static class ProducerConfigurationBuilderExtensions
    {
        /// <summary>
        /// Configures the produce throttling
        /// </summary>
        /// <param name="middlewares">Instance of <see cref="IProducerMiddlewareConfigurationBuilder"/></param>
        /// <param name="throttling">A handle to configure the throttling</param>
        /// <returns></returns>
        public static IProducerMiddlewareConfigurationBuilder WithThrottling(
            this IProducerMiddlewareConfigurationBuilder middlewares,
            Action<IProducerThrottlingBuilder> throttling)
        {
            var throttlingBuilder = new ProducerThrottlingBuilder();

            throttling(throttlingBuilder);

            foreach (var evalBuilder in throttlingBuilder.EvaluationBuilders)
            {
                middlewares.Add(resolver => new ProducerThrottlingMiddleware(evalBuilder.Build(resolver)));
            }

            return middlewares;
        }
    }
}
