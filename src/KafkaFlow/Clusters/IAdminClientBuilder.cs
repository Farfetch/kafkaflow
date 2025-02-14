using System;
using Confluent.Kafka;

namespace KafkaFlow.Clusters;

/// <inheritdoc cref="AdminClientBuilder"/>
public interface IAdminClientBuilder
{
    /// <inheritdoc cref="AdminClientBuilder.SetOAuthBearerTokenRefreshHandler"/>
    IAdminClientBuilder SetOAuthBearerTokenRefreshHandler(
        Action<IProducer<Null, Null>, string> oAuthBearerTokenRefreshHandler);

    /// <inheritdoc cref="AdminClientBuilder.Build"/>
    IAdminClient Build();
}