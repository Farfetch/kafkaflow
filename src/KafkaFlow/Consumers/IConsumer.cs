using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KafkaFlow.Configuration;

namespace KafkaFlow.Consumers;

/// <summary>
/// Represents a KafkaFlow consumer
/// </summary>
public interface IConsumer : IDisposable
{
    /// <summary>
    /// Gets the consumer configuration
    /// </summary>
    IConsumerConfiguration Configuration { get; }

    /// <inheritdoc cref="Confluent.Kafka.IConsumer{TKey,TValue}.Subscription"/>
    IReadOnlyList<string> Subscription { get; }

    /// <inheritdoc cref="Confluent.Kafka.IConsumer{TKey,TValue}.Assignment"/>
    IReadOnlyList<Confluent.Kafka.TopicPartition> Assignment { get; }

    /// <summary>
    /// Gets the consumer <see cref="IConsumerFlowManager"/>
    /// </summary>
    IConsumerFlowManager FlowManager { get; }

    /// <inheritdoc cref="Confluent.Kafka.IConsumer{TKey,TValue}.MemberId"/>
    string MemberId { get; }

    /// <inheritdoc cref="Confluent.Kafka.IClient.Name"/>
    string ClientInstanceName { get; }

    /// <summary>
    /// Gets the current consumer status
    /// </summary>
    ConsumerStatus Status { get; }

    /// <summary>
    /// Gets the lag of each topic/partitions assigned
    /// </summary>
    /// <returns>The list of topic, partition and lag</returns>
    IEnumerable<TopicPartitionLag> GetTopicPartitionsLag();

    /// <summary>
    /// Register a handler to be executed when the partitions are assigned
    /// </summary>
    /// <param name="handler">The handler that will be executed</param>
    void OnPartitionsAssigned(Action<IDependencyResolver, Confluent.Kafka.IConsumer<byte[], byte[]>, List<Confluent.Kafka.TopicPartition>> handler);

    /// <summary>
    /// Register a handler to be executed when the partitions are revoked
    /// </summary>
    /// <param name="handler">The handler that will be executed</param>
    void OnPartitionsRevoked(Action<IDependencyResolver, Confluent.Kafka.IConsumer<byte[], byte[]>, List<Confluent.Kafka.TopicPartitionOffset>> handler);

    /// <summary>
    /// Register a handler to be executed when an error occurs
    /// </summary>
    /// <param name="handler">The handler that will be executed</param>
    void OnError(Action<Confluent.Kafka.IConsumer<byte[], byte[]>, Confluent.Kafka.Error> handler);

    /// <summary>
    /// Register a handler to be executed to receive statistics information
    /// </summary>
    /// <param name="handler">The handler that will be executed</param>
    void OnStatistics(Action<Confluent.Kafka.IConsumer<byte[], byte[]>, string> handler);

    /// <inheritdoc cref="Confluent.Kafka.IConsumer{TKey,TValue}.Position"/>
    Confluent.Kafka.Offset GetPosition(Confluent.Kafka.TopicPartition topicPartition);

    /// <inheritdoc cref="Confluent.Kafka.IConsumer{TKey,TValue}.GetWatermarkOffsets"/>
    Confluent.Kafka.WatermarkOffsets GetWatermarkOffsets(Confluent.Kafka.TopicPartition topicPartition);

    /// <inheritdoc cref="Confluent.Kafka.IConsumer{TKey,TValue}.QueryWatermarkOffsets"/>
    Confluent.Kafka.WatermarkOffsets QueryWatermarkOffsets(Confluent.Kafka.TopicPartition topicPartition, TimeSpan timeout);

    /// <inheritdoc cref="Confluent.Kafka.IConsumer{TKey,TValue}.OffsetsForTimes"/>
    List<Confluent.Kafka.TopicPartitionOffset> OffsetsForTimes(
        IEnumerable<Confluent.Kafka.TopicPartitionTimestamp> topicPartitions,
        TimeSpan timeout);

    /// <inheritdoc cref="Confluent.Kafka.IConsumer{TKey,TValue}.Commit()"/>
    void Commit(IReadOnlyCollection<Confluent.Kafka.TopicPartitionOffset> offsetsValues);

    /// <inheritdoc cref="Confluent.Kafka.IConsumer{TKey,TValue}.Consume(CancellationToken)"/>
    ValueTask<Confluent.Kafka.ConsumeResult<byte[], byte[]>> ConsumeAsync(CancellationToken cancellationToken);
}
