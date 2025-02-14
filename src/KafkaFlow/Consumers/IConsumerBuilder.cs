using System;
using System.Collections.Generic;
using Confluent.Kafka;

namespace KafkaFlow.Consumers;

/// <inheritdoc cref="ConsumerBuilder{TKey,TValue}"/>
public interface IConsumerBuilder
{
    /// <inheritdoc cref="ConsumerBuilder{TKey,TValue}.SetPartitionsAssignedHandler(Action{IConsumer{TKey,TValue},List{TopicPartition}})"/>
    IConsumerBuilder SetPartitionsAssignedHandler(
        Action<IConsumer<byte[], byte[]>, List<TopicPartition>> partitionAssignmentHandler);

    /// <inheritdoc cref="ConsumerBuilder{TKey,TValue}.SetPartitionsRevokedHandler(Action{IConsumer{TKey,TValue},List{Confluent.Kafka.TopicPartitionOffset}})"/>
    IConsumerBuilder SetPartitionsRevokedHandler(
        Action<IConsumer<byte[], byte[]>, List<Confluent.Kafka.TopicPartitionOffset>> partitionsRevokedHandler);

    /// <inheritdoc cref="ConsumerBuilder{TKey,TValue}.SetErrorHandler"/>
    IConsumerBuilder SetErrorHandler(
        Action<IConsumer<byte[], byte[]>, Error> errorHandler);

    /// <inheritdoc cref="ConsumerBuilder{TKey,TValue}.SetStatisticsHandler"/>
    IConsumerBuilder SetStatisticsHandler(
        Action<IConsumer<byte[], byte[]>, string> statisticsHandler);

    /// <inheritdoc cref="ConsumerBuilder{TKey,TValue}.SetOAuthBearerTokenRefreshHandler"/>
    IConsumerBuilder SetOAuthBearerTokenRefreshHandler(
        Action<IConsumer<byte[], byte[]>, string> oAuthBearerTokenRefreshHandler);

    /// <inheritdoc cref="ConsumerBuilder{TKey,TValue}.Build"/>
    IConsumer<byte[], byte[]> Build();
}