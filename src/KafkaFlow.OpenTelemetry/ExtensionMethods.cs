namespace KafkaFlow.Configuration
{
    using KafkaFlow.OpenTelemetry;

    /// <summary>
    /// Adds OpenTelemetry instrumentation
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Adds OpenTelemetry instrumentation
        /// </summary>
        /// <param name="builder">The Kafka configuration builder</param>
        /// <returns></returns>
        public static IKafkaConfigurationBuilder AddOpenTelemetryInstrumentation(this IKafkaConfigurationBuilder builder)
        {
            // Todo: Missing on exception and on producer completed
            builder.SubscribeGlobalEvents(hub =>
            {
                hub.MessageConsumeStarted.Subscribe(eventContext =>
                {
                    return OpenTelemetryConsumerObserver.OnConsumeStarted(eventContext.MessageContext);
                });

                hub.MessageConsumeCompleted.Subscribe(eventContext =>
                {
                    return OpenTelemetryConsumerObserver.OnConsumeCompleted(eventContext.MessageContext);
                });

                hub.MessageProduceStarted.Subscribe(eventContext =>
                {
                    return OpenTelemetryProducerObserver.OnProducerStarted(eventContext.MessageContext);
                });
            });

            return builder;
        }
    }
}
