namespace KafkaFlow;

/// <summary>
/// Represents a Kafka topic and partition pair.
/// </summary>
public readonly struct TopicPartition
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TopicPartition"/> struct with the specified topic name, partition number, and offset value.
    /// </summary>
    /// <param name="topic">The name of the topic.</param>
    /// <param name="partition">The id of the partition.</param>
    public TopicPartition(string topic, int partition)
    {
        Topic = topic;
        Partition = partition;
    }

    /// <summary>
    /// Gets the name of the topic.
    /// </summary>
    public string Topic { get; }

    /// <summary>
    /// Gets the id of the partition.
    /// </summary>
    public int Partition { get; }
}
