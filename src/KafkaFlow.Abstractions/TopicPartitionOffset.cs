namespace KafkaFlow;

/// <summary>
/// Represents a Kafka topic along with its partition and offset information.
/// </summary>
public readonly struct TopicPartitionOffset
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TopicPartitionOffset"/> struct with the specified topic name, partition number, and offset value.
    /// </summary>
    /// <param name="topic">The name of the topic.</param>
    /// <param name="partition">The id of the partition.</param>
    /// <param name="offset">The offset value.</param>
    public TopicPartitionOffset(string topic, int partition, long offset)
    {
        this.Topic = topic;
        this.Partition = partition;
        this.Offset = offset;
    }

    /// <summary>
    /// Gets the name of the topic.
    /// </summary>
    public string Topic { get; }

    /// <summary>
    /// Gets the id of the partition.
    /// </summary>
    public int Partition { get; }

    /// <summary>
    /// Gets the offset value.
    /// </summary>
    public long Offset { get; }
}
