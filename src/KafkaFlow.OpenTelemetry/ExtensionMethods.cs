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
            var openTelemetryConsumerObserver = new OpenTelemetryConsumerObserver();
            var openTelemetryProducerObserver = new OpenTelemetryProducerObserver();

            builder.SubscribeGlobalEvents(hub =>
            {
                hub.MessageConsumeStarted.Subscribe(eventContext =>
                {
                    var observer = new OpenTelemetryConsumerObserver();
                    return observer.OnConsumeStarted(eventContext.MessageContext);
                });

                hub.MessageProduceStarted.Subscribe(eventContext =>
                {
                    var observer = new OpenTelemetryProducerObserver();
                    return observer.OnProducerStarted(eventContext.MessageContext);
                });
            });

            return builder;
        }
    }
}
