using System.Collections.Generic;
using Confluent.Kafka;

namespace KafkaFlow.Authentication;

internal readonly struct OAuthBearerAuthenticator : IOAuthBearerAuthenticator
{
    private readonly IClient _client;

    public OAuthBearerAuthenticator(IClient client)
    {
        _client = client;
    }

    public void SetToken(string tokenValue, long lifetimeMs, string principalName, IDictionary<string, string> extensions = null)
    {
        _client.OAuthBearerSetToken(tokenValue, lifetimeMs, principalName, extensions);
    }

    public void SetTokenFailure(string error)
    {
        _client.OAuthBearerSetTokenFailure(error);
    }
}
