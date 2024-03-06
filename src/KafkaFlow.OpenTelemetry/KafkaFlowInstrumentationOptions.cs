using System;
using System.Diagnostics;

namespace KafkaFlow.OpenTelemetry;

/// <summary>
/// The options to be included in KafkaFlow instrumentation
/// </summary>
public class KafkaFlowInstrumentationOptions
{
    /// <summary>
    /// Gets or sets the Producer enricher
    /// </summary>
    public Action<Activity, IMessageContext> EnrichProducer { get; set; }

    /// <summary>
    /// Gets or sets the Consumer enricher
    /// </summary>
    public Action<Activity, IMessageContext> EnrichConsumer { get; set; }
}
