namespace KafkaFlow
{
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
