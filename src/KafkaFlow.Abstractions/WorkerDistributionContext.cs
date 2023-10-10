namespace KafkaFlow;

using System;
using System.Threading;

/// <summary>
/// Represents a strategy context for distributing workers based on specific message and consumer details.
/// </summary>
public ref struct WorkerDistributionContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkerDistributionContext"/> struct.
    /// </summary>
    /// <param name="consumerName">Name of the consumer.</param>
    /// <param name="topic">Topic associated with the message.</param>
    /// <param name="partition">Partition of the topic.</param>
    /// <param name="rawMessageKey">Raw key of the message.</param>
    /// <param name="consumerStoppedCancellationToken">A cancellation token that is cancelled when the consumer has stopped</param>
    public WorkerDistributionContext(
        string consumerName,
        string topic,
        int partition,
        ReadOnlyMemory<byte>? rawMessageKey,
        CancellationToken consumerStoppedCancellationToken)
    {
        this.ConsumerName = consumerName;
        this.Topic = topic;
        this.Partition = partition;
        this.RawMessageKey = rawMessageKey;
        this.ConsumerStoppedCancellationToken = consumerStoppedCancellationToken;
    }

    /// <summary>
    /// Gets the name of the consumer.
    /// </summary>
    public string ConsumerName { get; }

    /// <summary>
    /// Gets the topic associated with the message.
    /// </summary>
    public string Topic { get; }

    /// <summary>
    /// Gets the partition number of the topic.
    /// </summary>
    public int Partition { get; }

    /// <summary>
    /// Gets the raw key of the message.
    /// </summary>
    public ReadOnlyMemory<byte>? RawMessageKey { get; }

    /// <summary>
    /// Gets the cancellation token that is cancelled when the consumer has stopped
    /// </summary>
    public CancellationToken ConsumerStoppedCancellationToken { get; }
}
