namespace KafkaFlow.Configuration
{
    using KafkaFlow.OpenTelemetry;
    using KafkaFlow.OpenTelemetry.Trace;

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
                hub.MessageConsumeStarted.Subscribe(openTelemetryConsumerObserver);
            });

            //builder.SubscribeConsumerEvents(openTelemetryConsumerObserver);
            //builder.SubscribeProducerInstrumentationSubjects(openTelemetryProducerObserver);

            return builder;
        }
    }
}
