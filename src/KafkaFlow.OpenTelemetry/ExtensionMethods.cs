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
            builder.SubscribeGlobalEvents(hub =>
            {
                hub.MessageConsumeStarted.Subscribe(eventContext =>
                {
                    return OpenTelemetryConsumerObserver.OnConsumeStarted(eventContext.MessageContext);
                });

                hub.MessageConsumeError.Subscribe(eventContext =>
                {
                    return OpenTelemetryConsumerObserver.OnConsumeError(eventContext.MessageContext, eventContext.Exception);
                });

                hub.MessageConsumeCompleted.Subscribe(eventContext =>
                {
                    return OpenTelemetryConsumerObserver.OnConsumeCompleted(eventContext.MessageContext);
                });

                hub.MessageProduceStarted.Subscribe(eventContext =>
                {
                    return OpenTelemetryProducerObserver.OnProducerStarted(eventContext.MessageContext);
                });

                hub.MessageProduceError.Subscribe(eventContext =>
                {
                    return OpenTelemetryProducerObserver.OnProducerError(eventContext.MessageContext, eventContext.Exception);
                });

                hub.MessageProduceCompleted.Subscribe(eventContext =>
                {
                    return OpenTelemetryProducerObserver.OnProducerCompleted(eventContext.MessageContext);
                });
            });

            return builder;
        }
    }
}
