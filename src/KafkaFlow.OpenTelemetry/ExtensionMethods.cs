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
                    return OpenTelemetryConsumerEventsHandler.OnConsumeStarted(eventContext.MessageContext);
                });

                hub.MessageConsumeError.Subscribe(eventContext =>
                {
                    return OpenTelemetryConsumerEventsHandler.OnConsumeError(eventContext.MessageContext, eventContext.Exception);
                });

                hub.MessageConsumeCompleted.Subscribe(eventContext =>
                {
                    return OpenTelemetryConsumerEventsHandler.OnConsumeCompleted(eventContext.MessageContext);
                });

                hub.MessageProduceStarted.Subscribe(eventContext =>
                {
                    return OpenTelemetryProducerEventsHandler.OnProducerStarted(eventContext.MessageContext);
                });

                hub.MessageProduceError.Subscribe(eventContext =>
                {
                    return OpenTelemetryProducerEventsHandler.OnProducerError(eventContext.MessageContext, eventContext.Exception);
                });

                hub.MessageProduceCompleted.Subscribe(eventContext =>
                {
                    return OpenTelemetryProducerEventsHandler.OnProducerCompleted(eventContext.MessageContext);
                });
            });

            return builder;
        }
    }
}
