namespace KafkaFlow
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the message consumer
    /// </summary>
    public interface IConsumerContext
    {
        /// <summary>
        /// Gets the consumer unique name defined in configuration
        /// </summary>
        string ConsumerName { get; }

        /// <summary>
        /// Gets a CancellationToken that is cancelled when the worker is requested to stop
        /// </summary>
        CancellationToken WorkerStopped { get; }

        /// <summary>
        /// Gets the worker id that is processing the message
        /// </summary>
        int WorkerId { get; }

        /// <summary>
        /// Gets the topic associated with the message
        /// </summary>
        string Topic { get; }

        /// <summary>
        /// Gets the partition associated with the message
        /// </summary>
        int Partition { get; }

        /// <summary>
        /// Gets the partition offset associated with the message
        /// </summary>
        long Offset { get; }

        /// <summary>
        /// Gets the <see cref="TopicPartitionOffset"/> object associated with the message
        /// </summary>
        TopicPartitionOffset TopicPartitionOffset { get; }

        /// <summary>
        /// Gets the consumer group id from kafka consumer that received the message
        /// </summary>
        string GroupId { get; }

        /// <summary>
        /// Gets message timestamp. By default is the UTC timestamp when the message was produced
        /// </summary>
        DateTime MessageTimestamp { get; }

        /// <summary>
        /// Gets or sets a value indicating whether if the framework should invoke the <see cref="Complete"/> method after the message has been processed
        /// </summary>
        bool AutoMessageCompletion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether if the message offset must be stored when the message is marked as completed
        /// </summary>
        bool ShouldStoreOffset { get; set; }

        /// <summary>
        /// Gets an instance of IDependencyResolver which provides methods to resolve dependencies.
        /// This instance is tied to the consumer scope, meaning it is capable of resolving dependencies
        /// that are scoped to the lifecycle of a single consumer.
        /// </summary>
        IDependencyResolver ConsumerDependencyResolver { get; }

        /// <summary>
        /// Gets an instance of IDependencyResolver which provides methods to resolve dependencies.
        /// This instance is tied to the worker scope, meaning it is capable of resolving dependencies
        /// that are scoped to the lifecycle of a single worker.
        /// </summary>
        IDependencyResolver WorkerDependencyResolver { get; }

        /// <summary>
        /// Gets a Task that completes when the <see cref="Complete"/> method is invoked,
        /// indicating the end of message processing. This allows async operations
        /// to wait for the message to be fully processed and its offset stored.
        /// </summary>
        Task<TopicPartitionOffset> Completion { get; }

        /// <summary>
        /// Signals the completion of message processing and stores the message offset to eventually be committed.
        /// After this call, the framework marks the message processing as finished and releases resources associated with the message.
        /// By default, this method is automatically invoked when message processing concludes, unless
        /// the consumer is configured for manual message completion or the <see cref="AutoMessageCompletion"/> flag is set to false.
        /// </summary>
        void Complete(IMessageContext context);

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
