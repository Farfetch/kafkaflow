namespace KafkaFlow;

/// <summary>
/// Represents the interface of a internal worker
/// </summary>
public interface IWorker
{
    /// <summary>
    /// Gets worker's id
    /// </summary>
    int Id { get; }

    /// <summary>
    /// Gets the subject for worker stopping events where observers can subscribe to receive notifications.
    /// </summary>
    IEvent WorkerStopping { get; }

    /// <summary>
    /// Gets the subject for worker stopped events where observers can subscribe to receive notifications.
    /// </summary>
    IEvent WorkerStopped { get; }

    /// <summary>
    /// Gets the subject for worker consumption completed events where observers can subscribe to receive notifications.
    /// </summary>
    IEvent<IMessageContext> WorkerProcessingEnded { get; }
}
