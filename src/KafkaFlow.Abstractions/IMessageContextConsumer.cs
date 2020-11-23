namespace KafkaFlow
{
    using System;
    using System.Threading;

    /// <summary>
    /// Represents the message consumer
    /// </summary>
    public interface IMessageContextConsumer
    {
        /// <summary>
        /// The consumer unique name defined in configuration
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A CancellationToken that is cancelled when the worker is requested to stop
        /// </summary>
        CancellationToken WorkerStopped { get; }

        /// <summary>
        /// Message timestamp. By default is the UTC timestamp when the message was produced
        /// </summary>
        DateTime MessageTimestamp { get; }

        /// <summary>
        /// Gets or Sets if the framework should store the current offset in the end when auto store offset is used
        /// </summary>
        bool ShouldStoreOffset { get; set; }

        /// <summary>
        /// Store the message offset when manual store option is used
        /// </summary>
        void StoreOffset();

        /// <summary>
        /// Get offset watermark data
        /// </summary>
        /// <returns></returns>
        IOffsetsWatermark GetOffsetsWatermark();

        /// <summary>
        /// Pause Kafka's message fetch, buffered messages will still be processed
        /// </summary>
        void Pause();

        /// <summary>
        /// Resume Kafka's message fetch
        /// </summary>
        void Resume();
    }
}
