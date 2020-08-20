namespace KafkaFlow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using KafkaFlow.Configuration;
    using KafkaFlow.Consumers;
    using KafkaFlow.Producers.Middlewares.Throttling;
    using KafkaFlow.Producers.Middlewares.Throttling.Actions;
    using KafkaFlow.Producers.Middlewares.Throttling.Configuration;
    using KafkaFlow.Producers.Middlewares.Throttling.Evaluations;

    /// <summary>
    /// No needed
    /// </summary>
    public static class ProducerConfigurationBuilderExtensions
    {
        /// <summary>
        /// Adds the producer throttling middleware
        /// </summary>
        /// <param name="middlewares">Instance of <see cref="IProducerMiddlewareConfigurationBuilder"/></param>
        /// <param name="throttling">A handle to configure the throttling</param>
        /// <returns></returns>
        public static IProducerMiddlewareConfigurationBuilder AddThrottling(
            this IProducerMiddlewareConfigurationBuilder middlewares,
            Action<IProducerThrottlingBuilder> throttling)
        {
            var throttlingBuilder = new ProducerThrottlingBuilder();

            throttling(throttlingBuilder);

            foreach (var evalBuilder in throttlingBuilder.EvaluationBuilders)
            {
                middlewares.Add(
                    (resolver, configuration) =>
                        new ProducerThrottlingMiddleware(
                            resolver.Resolve<IKafkaBus>(),
                            configuration,
                            evalBuilder.Build(resolver)));
            }

            return middlewares;
        }

        public static IProducerEvaluationBuilder AddLagEvaluation(
            this IProducerThrottlingBuilder builder,
            TimeSpan interval,
            string topic,
            IReadOnlyCollection<string> consumerGroups)
        {
            return builder.AddEvaluation(
                interval,
                resolver => new LagEvaluation(
                    resolver.Resolve<IClusterAccessor>(),
                    topic,
                    consumerGroups));
        }

        public static IProducerEvaluationBuilder AddLagEvaluation(
            this IProducerThrottlingBuilder builder,
            TimeSpan interval,
            string topic,
            params string[] consumerGroups) =>
            builder.AddLagEvaluation(interval, topic, consumerGroups.ToList());

        public static IProducerEvaluationBuilder AddDelayAction(
            this IProducerEvaluationBuilder builder,
            int threshold,
            TimeSpan delay)
        {
            return builder.AddAction(resolver => new DelayAction(threshold, delay));
        }

        public static IProducerEvaluationBuilder AddPauseConsumerAction(
            this IProducerEvaluationBuilder builder,
            int threshold,
            string consumerName)
        {
            return builder.AddAction(
                resolver => new PauseConsumerAction(
                    resolver.Resolve<IConsumerAccessor>(),
                    threshold,
                    consumerName));
        }
    }
}
