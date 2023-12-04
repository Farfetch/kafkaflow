namespace KafkaFlow.Consumers;

/// <summary>
/// Represents the lag in a specific topic and partition
/// </summary>
public class TopicPartitionLag
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TopicPartitionLag"/> class.
    /// </summary>
    /// <param name="topic">The topic name</param>
    /// <param name="partition">The partition value</param>
    /// <param name="lag">The lag value</param>
    public TopicPartitionLag(string topic, int partition, long lag)
    {
        this.Topic = topic;
        this.Partition = partition;
        this.Lag = lag;
    }

    /// <summary>
    /// Gets the topic name
    /// </summary>
    public string Topic { get; }

    /// <summary>
    /// Gets the partition value
    /// </summary>
    public int Partition { get; }

    /// <summary>
    /// Gets the lag
    /// </summary>
    public long Lag { get; }
}
