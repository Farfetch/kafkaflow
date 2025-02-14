using System;
using Confluent.Kafka;

namespace KafkaFlow.Producers;

/// <inheritdoc cref="ProducerBuilder{TKey,TValue}"/>
public interface IProducerBuilder
{
    /// <inheritdoc cref="ProducerBuilder{TKey,TValue}.SetErrorHandler"/>
    IProducerBuilder SetErrorHandler(
        Action<IProducer<byte[], byte[]>, Error> errorHandler);

    /// <inheritdoc cref="ProducerBuilder{TKey,TValue}.SetStatisticsHandler"/>
    IProducerBuilder SetStatisticsHandler(
        Action<IProducer<byte[], byte[]>, string> statisticsHandler);

    /// <inheritdoc cref="ProducerBuilder{TKey,TValue}.SetOAuthBearerTokenRefreshHandler"/>
    IProducerBuilder SetOAuthBearerTokenRefreshHandler(
        Action<IProducer<byte[], byte[]>, string> oAuthBearerTokenRefreshHandler);

    /// <inheritdoc cref="ProducerBuilder{TKey,TValue}.Build"/>
    IProducer<byte[], byte[]> Build();
}