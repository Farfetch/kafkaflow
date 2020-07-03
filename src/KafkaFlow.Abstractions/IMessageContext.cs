namespace KafkaFlow
{
    /// <summary>
    /// Represents the context of a message from a Kafka cluster
    /// </summary>
    public interface IMessageContext
    {
        /// <summary>
        /// Gets the worker id that processed the message
        /// </summary>
        int WorkerId { get; }

        /// <summary>
        /// Gets the message key
        /// </summary>
        byte[] PartitionKey { get; }

        /// <summary>
        /// Gets the message value
        /// </summary>
        object Message { get; }

        /// <summary>
        /// Gets the message headers
        /// </summary>
        IMessageHeaders Headers { get; }

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
        /// Gets the consumer group id from kafka consumer that received the message
        /// </summary>
        string GroupId { get; }

        /// <summary>
        /// Gets the <see cref="IMessageContextConsumer"></see> from the message consumed
        /// </summary>
        IMessageContextConsumer Consumer { get; }

        /// <summary>
        /// Updates the message value with the value provided
        /// </summary>
        /// <param name="message">New message value</param>
        void TransformMessage(object message);

        /// <summary>
        /// Creates a clone of the current <see cref="IMessageContext"></see>
        /// </summary>
        /// <returns>
        /// A clone of the current <see cref="IMessageContext"></see>
        /// </returns>
        IMessageContext Clone();
    }
}
