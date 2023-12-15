using System.Collections.Generic;
using Confluent.Kafka;

namespace KafkaFlow.Authentication;

internal class OAuthBearerAuthenticator : IOAuthBearerAuthenticator
{
    internal IClient Client { get; set; }

    public void SetToken(string tokenValue, long lifetimeMs, string principalName, IDictionary<string, string> extensions = null)
    {
        Client.OAuthBearerSetToken(tokenValue, lifetimeMs, principalName, extensions);
    }

    public void SetTokenFailure(string error)
    {
        Client.OAuthBearerSetTokenFailure(error);
    }
}