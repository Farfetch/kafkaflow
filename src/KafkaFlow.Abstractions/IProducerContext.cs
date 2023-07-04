namespace KafkaFlow;

/// <summary>
/// Some producer metadata
/// </summary>
public interface IProducerContext
{
    /// <summary>
    /// Gets the topic associated with the message
    /// </summary>
    string Topic { get; }

    /// <summary>
    /// Gets the partition associated with the message
    /// </summary>
    int? Partition { get; }

    /// <summary>
    /// Gets the partition offset associated with the message
    /// </summary>
    long? Offset { get; }

    /// <summary>
    /// Gets an instance of IDependencyResolver which provides methods to resolve dependencies.
    /// This instance is tied to the producer scope, meaning it is capable of resolving dependencies
    /// that are scoped to the lifecycle of a single producer.
    /// </summary>
    IDependencyResolver DependencyResolver { get; }
}
