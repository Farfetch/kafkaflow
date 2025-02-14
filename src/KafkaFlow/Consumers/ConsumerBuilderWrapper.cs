using System;
using System.Collections.Generic;
using Confluent.Kafka;

namespace KafkaFlow.Consumers;

internal class ConsumerBuilderWrapper : IConsumerBuilder
{
    private readonly ConsumerBuilder<byte[], byte[]> _builder;

    public ConsumerBuilderWrapper(ConsumerConfig config)
    {
        _builder = new ConsumerBuilder<byte[], byte[]>(config);
    }

    public IConsumerBuilder SetPartitionsAssignedHandler(Action<IConsumer<byte[], byte[]>, List<TopicPartition>> partitionAssignmentHandler)
    {
        this._builder.SetPartitionsAssignedHandler(partitionAssignmentHandler);

        return this;
    }

    public IConsumerBuilder SetPartitionsRevokedHandler(Action<IConsumer<byte[], byte[]>, List<Confluent.Kafka.TopicPartitionOffset>> partitionsRevokedHandler)
    {
        this._builder.SetPartitionsRevokedHandler(partitionsRevokedHandler);

        return this;
    }

    public IConsumerBuilder SetErrorHandler(Action<IConsumer<byte[], byte[]>, Error> errorHandler)
    {
        this._builder.SetErrorHandler(errorHandler);

        return this;
    }

    public IConsumerBuilder SetStatisticsHandler(Action<IConsumer<byte[], byte[]>, string> statisticsHandler)
    {
        this._builder.SetStatisticsHandler(statisticsHandler);

        return this;
    }

    public IConsumerBuilder SetOAuthBearerTokenRefreshHandler(Action<IConsumer<byte[], byte[]>, string> oAuthBearerTokenRefreshHandler)
    {
        this._builder.SetOAuthBearerTokenRefreshHandler(oAuthBearerTokenRefreshHandler);

        return this;
    }

    public IConsumer<byte[], byte[]> Build()
    {
        return this._builder.Build();
    }
}