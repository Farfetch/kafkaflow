using Confluent.Kafka;

namespace KafkaFlow.Producers;

/// <summary>
/// Represents a message to be produced in batch
/// </summary>
public class BatchProduceItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BatchProduceItem"/> class.
    /// </summary>
    /// <param name="topic">The destination topic</param>
    /// <param name="messageKey">The message partition key</param>
    /// <param name="messageValue">The message content</param>
    /// <param name="headers">The message headers</param>
    /// <param name="partition">The partition to produce the message to. If null, the partitioner will be used.</param>
    public BatchProduceItem(
        string topic,
        object messageKey,
        object messageValue,
        IMessageHeaders headers,
        int? partition = null)
    {
        this.Topic = topic;
        this.MessageKey = messageKey;
        this.MessageValue = messageValue;
        this.Headers = headers;
        this.Partition = partition;
    }

    /// <summary>
    /// Gets the message topic name
    /// </summary>
    public string Topic { get; }

    /// <summary>
    /// Gets the message partition key
    /// </summary>
    public object MessageKey { get; }

    /// <summary>
    /// Gets the message object
    /// </summary>
    public object MessageValue { get; }

    /// <summary>
    /// Gets the message headers
    /// </summary>
    public IMessageHeaders Headers { get; }

    /// <summary>
    /// Gets the delivery report after the production
    /// </summary>
    public DeliveryReport<byte[], byte[]> DeliveryReport { get; internal set; }

    /// <summary>
    /// Gets the partition to produce the message to. If null, the partitioner will decide the partition.
    /// </summary>
    public int? Partition { get; }
}
