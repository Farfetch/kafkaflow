namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using KafkaFlow.Configuration;

    /// <summary>
    /// Represents a KafkaFlow consumer
    /// </summary>
    public interface IConsumer : IDisposable
    {
        /// <summary>
        /// Gets the consumer configuration
        /// </summary>
        IConsumerConfiguration Configuration { get; }

        /// <inheritdoc cref="IConsumer{TKey,TValue}.Subscription"/>
        IReadOnlyList<string> Subscription { get; }

        /// <inheritdoc cref="IConsumer{TKey,TValue}.Assignment"/>
        IReadOnlyList<TopicPartition> Assignment { get; }

        /// <summary>
        /// Gets the consumer <see cref="IConsumerFlowManager"/>
        /// </summary>
        IConsumerFlowManager FlowManager { get; }

        /// <inheritdoc cref="IConsumer{TKey,TValue}.MemberId"/>
        string MemberId { get; }

        /// <inheritdoc cref="IClient.Name"/>
        string ClientInstanceName { get; }

        /// <summary>
        /// Gets the current consumer status
        /// </summary>
        ConsumerStatus Status { get; }

        /// <summary>
        /// Register a handler to be executed when the partitions are assigned
        /// </summary>
        /// <param name="handler">The handler that will be executed</param>
        void OnPartitionsAssigned(Action<IDependencyResolver, IConsumer<byte[], byte[]>, List<TopicPartition>> handler);

        /// <summary>
        /// Register a handler to be executed when the partitions are revoked
        /// </summary>
        /// <param name="handler">The handler that will be executed</param>
        void OnPartitionsRevoked(Action<IDependencyResolver, IConsumer<byte[], byte[]>, List<TopicPartitionOffset>> handler);

        /// <summary>
        /// Register a handler to be executed when an error occurs
        /// </summary>
        /// <param name="handler">The handler that will be executed</param>
        void OnError(Action<IConsumer<byte[], byte[]>, Error> handler);

        /// <summary>
        /// Register a handler to be executed to receive statistics information
        /// </summary>
        /// <param name="handler">The handler that will be executed</param>
        void OnStatistics(Action<IConsumer<byte[], byte[]>, string> handler);

        /// <inheritdoc cref="IConsumer{TKey,TValue}.Position"/>
        Offset GetPosition(TopicPartition topicPartition);

        /// <inheritdoc cref="IConsumer{TKey,TValue}.GetWatermarkOffsets"/>
        WatermarkOffsets GetWatermarkOffsets(TopicPartition topicPartition);

        /// <inheritdoc cref="IConsumer{TKey,TValue}.QueryWatermarkOffsets"/>
        WatermarkOffsets QueryWatermarkOffsets(TopicPartition topicPartition, TimeSpan timeout);

        /// <inheritdoc cref="IConsumer{TKey,TValue}.OffsetsForTimes"/>
        List<TopicPartitionOffset> OffsetsForTimes(
            IEnumerable<TopicPartitionTimestamp> topicPartitions,
            TimeSpan timeout);

        /// <inheritdoc cref="IConsumer{TKey,TValue}.Commit()"/>
        void Commit(IEnumerable<TopicPartitionOffset> offsetsValues);

        /// <inheritdoc cref="IConsumer{TKey,TValue}.Consume(CancellationToken)"/>
        ValueTask<ConsumeResult<byte[], byte[]>> ConsumeAsync(CancellationToken cancellationToken);
    }
}
