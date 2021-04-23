namespace KafkaFlow.Producers
{
    using Confluent.Kafka;

    /// <summary>
    /// Represents a message to be produced in batch
    /// </summary>
    public class BatchProduceItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BatchProduceItem"/> class.
        /// </summary>
        /// <param name="topic">The destination topic</param>
        /// <param name="partitionKey">The message partition key</param>
        /// <param name="message">The message content</param>
        /// <param name="headers">The message headers</param>
        public BatchProduceItem(
            string topic,
            string partitionKey,
            object message,
            IMessageHeaders headers)
        {
            this.Topic = topic;
            this.PartitionKey = partitionKey;
            this.Message = message;
            this.Headers = headers;
        }

        /// <summary>
        /// Gets the message topic name
        /// </summary>
        public string Topic { get; }

        /// <summary>
        /// Gets the message partition key
        /// </summary>
        public string PartitionKey { get; }

        /// <summary>
        /// Gets the message object
        /// </summary>
        public object Message { get; }

        /// <summary>
        /// Gets the message headers
        /// </summary>
        public IMessageHeaders Headers { get; }

        /// <summary>
        /// Gets the delivery report after the production
        /// </summary>
        public DeliveryReport<byte[], byte[]> DeliveryReport { get; internal set; }
    }
}
