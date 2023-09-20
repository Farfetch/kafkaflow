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
        public static IKafkaConfigurationBuilder AddOpenTelemetryInstrumentation(this IKafkaConfigurationBuilder builder)
        {
            var tracerConsumerMiddleware = new TracerConsumerMiddleware();
            var tracerProducerMiddleware = new TracerProducerMiddleware();

            builder.SubscribeEvents(events =>
            {
                events.OnConsumeError += (sender, args) => tracerConsumerMiddleware.UpdateActivityOnError(args.Exception);
                events.OnConsumeStart += (sender, args) => tracerConsumerMiddleware.CreateActivityOnConsume(args.MessageContext);
                events.OnProduceError += (sender, args) => tracerProducerMiddleware.UpdateActivityOnError(args.Exception);
                events.OnProduceStart += (sender, args) => tracerProducerMiddleware.CreateActivityOnProduce(args.MessageContext);
            });

            return builder;
        }
    }
}
