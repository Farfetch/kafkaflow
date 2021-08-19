namespace KafkaFlow
{
    /// <summary>
    /// A context that can have some metadata to help with serialization process
    /// </summary>
    public interface ISerializerContext
    {
        /// <summary>
        /// Gets the topic associated with the message
        /// </summary>
        string Topic { get; }
    }
}
