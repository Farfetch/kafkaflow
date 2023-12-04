using System.Collections.Generic;

namespace KafkaFlow.Configuration;

/// <summary>
/// A metadata class with some context information help to calculate the number of workers
/// </summary>
public class WorkersCountContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkersCountContext"/> class.
    /// </summary>
    /// <param name="consumerName">The consumer's name</param>
    /// <param name="consumerGroupId">The consumer's group id</param>
    /// <param name="assignedTopicsPartitions">The consumer's assigned partition</param>
    public WorkersCountContext(
        string consumerName,
        string consumerGroupId,
        IReadOnlyCollection<TopicPartitions> assignedTopicsPartitions)
    {
        this.ConsumerName = consumerName;
        this.ConsumerGroupId = consumerGroupId;
        this.AssignedTopicsPartitions = assignedTopicsPartitions;
    }

    /// <summary>
    /// Gets the consumer's name
    /// </summary>
    public string ConsumerName { get; }

    /// <summary>
    /// Gets the consumer's group id
    /// </summary>
    public string ConsumerGroupId { get; }

    /// <summary>
    /// Gets the assigned partitions to the consumer
    /// </summary>
    public IReadOnlyCollection<TopicPartitions> AssignedTopicsPartitions { get; }
}
