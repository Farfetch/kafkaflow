using System;
using Confluent.Kafka;

namespace KafkaFlow.Producers;

internal class ProducerBuilderWrapper : IProducerBuilder
{
    private readonly ProducerBuilder<byte[], byte[]> _builder;

    public ProducerBuilderWrapper(ProducerConfig config)
    {
        _builder = new ProducerBuilder<byte[], byte[]>(config);
    }

    public IProducerBuilder SetErrorHandler(Action<IProducer<byte[], byte[]>, Error> errorHandler)
    {
        this._builder.SetErrorHandler(errorHandler);

        return this;
    }

    public IProducerBuilder SetStatisticsHandler(Action<IProducer<byte[], byte[]>, string> statisticsHandler)
    {
        this._builder.SetStatisticsHandler(statisticsHandler);

        return this;
    }

    public IProducerBuilder SetOAuthBearerTokenRefreshHandler(Action<IProducer<byte[], byte[]>, string> oAuthBearerTokenRefreshHandler)
    {
        this._builder.SetOAuthBearerTokenRefreshHandler(oAuthBearerTokenRefreshHandler);

        return this;
    }

    public IProducer<byte[], byte[]> Build()
    {
        return this._builder.Build();
    }
}