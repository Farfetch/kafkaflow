namespace KafkaFlow
{
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
    }
}
