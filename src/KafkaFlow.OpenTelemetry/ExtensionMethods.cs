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
                hub.MessageConsumeStarted.Subscribe(eventContext => OpenTelemetryConsumerEventsHandler.OnConsumeStarted(eventContext.MessageContext));

                //hub.MessageConsumeError.Subscribe(eventContext => OpenTelemetryConsumerEventsHandler.OnConsumeError(eventContext.MessageContext, eventContext.Exception));

                //hub.MessageConsumeCompleted.Subscribe(eventContext => OpenTelemetryConsumerEventsHandler.OnConsumeCompleted(eventContext.MessageContext));

                hub.MessageProduceStarted.Subscribe(eventContext => OpenTelemetryProducerEventsHandler.OnProducerStarted(eventContext.MessageContext));

                hub.MessageProduceError.Subscribe(eventContext => OpenTelemetryProducerEventsHandler.OnProducerError(eventContext.MessageContext, eventContext.Exception));

                hub.MessageProduceCompleted.Subscribe(eventContext => OpenTelemetryProducerEventsHandler.OnProducerCompleted(eventContext.MessageContext));
            });

            return builder;
        }
    }
}
