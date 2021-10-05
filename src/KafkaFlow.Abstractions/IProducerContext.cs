namespace KafkaFlow
{
    using System.Threading;

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
        /// Gets a CancellationToken that is cancelled when the client producing the message is requested to stop
        /// </summary>
        CancellationToken ClientStopped { get; }
    }
}
