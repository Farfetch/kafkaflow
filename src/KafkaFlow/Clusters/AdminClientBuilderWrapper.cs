using System;
using Confluent.Kafka;

namespace KafkaFlow.Clusters;

internal class AdminClientBuilderWrapper : IAdminClientBuilder
{
    private readonly AdminClientBuilder _builder;

    public AdminClientBuilderWrapper(AdminClientConfig config)
    {
        _builder = new AdminClientBuilder(config);
    }

    public IAdminClientBuilder SetOAuthBearerTokenRefreshHandler(Action<IProducer<Null, Null>, string> oAuthBearerTokenRefreshHandler)
    {
        this._builder.SetOAuthBearerTokenRefreshHandler(oAuthBearerTokenRefreshHandler);

        return this;
    }

    public IAdminClient Build()
    {
        return this._builder.Build();
    }
}