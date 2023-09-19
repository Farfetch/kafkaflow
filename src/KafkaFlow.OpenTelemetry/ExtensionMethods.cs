namespace KafkaFlow.Configuration
{
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
        public static IClusterConfigurationBuilder AddOpenTelemetryInstrumentation(this IClusterConfigurationBuilder builder) =>
            builder.AddInstrumentation<TracerConsumerMiddleware, TracerProducerMiddleware>();
    }
}
