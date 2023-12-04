namespace KafkaFlow.Consumers;

/// <summary>
/// Provides access to the current consumer's middleware context.
/// </summary>
public interface IConsumerMiddlewareContext
{
    /// <summary>
    /// Gets the current worker in the context.
    /// This property only returns values when inside a middleware with Worker lifetime; otherwise, it will return null.
    /// </summary>
    IWorker Worker { get; }

    /// <summary>
    /// Gets the current consumer in the context.
    /// </summary>
    IConsumer Consumer { get; }
}
