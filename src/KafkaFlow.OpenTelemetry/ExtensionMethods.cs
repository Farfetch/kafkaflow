using System;
using KafkaFlow.OpenTelemetry;

namespace KafkaFlow.Configuration;

/// <summary>
/// Adds OpenTelemetry instrumentation
/// </summary>
public static class ExtensionMethods
{
    /// <summary>
    /// Adds OpenTelemetry instrumentation
    /// </summary>
    /// <param name="builder">The Kafka configuration builder</param>
    /// <param name="options">KafkaFlowInstrumentationOptions action</param>
    /// <returns></returns>
    public static IKafkaConfigurationBuilder AddOpenTelemetryInstrumentation(this IKafkaConfigurationBuilder builder, Action<KafkaFlowInstrumentationOptions> options)
    {
        var kafkaFlowInstrumentationOptions = new KafkaFlowInstrumentationOptions();

        options?.Invoke(kafkaFlowInstrumentationOptions);

        builder.SubscribeGlobalEvents(hub =>
        {
            hub.MessageConsumeStarted.Subscribe(eventContext => OpenTelemetryConsumerEventsHandler.OnConsumeStarted(eventContext.MessageContext, kafkaFlowInstrumentationOptions));

            hub.MessageConsumeError.Subscribe(eventContext => OpenTelemetryConsumerEventsHandler.OnConsumeError(eventContext.MessageContext, eventContext.Exception));

            hub.MessageConsumeCompleted.Subscribe(eventContext => OpenTelemetryConsumerEventsHandler.OnConsumeCompleted(eventContext.MessageContext));

            hub.MessageProduceStarted.Subscribe(eventContext => OpenTelemetryProducerEventsHandler.OnProducerStarted(eventContext.MessageContext, kafkaFlowInstrumentationOptions));

            hub.MessageProduceError.Subscribe(eventContext => OpenTelemetryProducerEventsHandler.OnProducerError(eventContext.MessageContext, eventContext.Exception));

            hub.MessageProduceCompleted.Subscribe(eventContext => OpenTelemetryProducerEventsHandler.OnProducerCompleted(eventContext.MessageContext));
        });

        return builder;
    }

    /// <summary>
    /// Adds OpenTelemetry instrumentation
    /// </summary>
    /// <param name="builder">The Kafka configuration builder</param>
    /// <returns></returns>
    public static IKafkaConfigurationBuilder AddOpenTelemetryInstrumentation(this IKafkaConfigurationBuilder builder)
    {
        return AddOpenTelemetryInstrumentation(builder, null);
    }
}
