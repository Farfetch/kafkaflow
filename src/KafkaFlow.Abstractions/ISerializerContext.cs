namespace KafkaFlow;

/// <summary>
/// Context for serialization and deserialization operations.
/// </summary>
public interface ISerializerContext
{
    /// <summary>
    /// Gets the topic associated with the message
    /// </summary>
    string Topic { get; }
}
