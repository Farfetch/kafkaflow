using System.Collections.Generic;

namespace KafkaFlow.Authentication;

/// <summary>
/// Authentication handler for OAuth Bearer.
/// </summary>
public interface IOAuthBearerAuthenticator
{
    /// <summary>
    ///     Set SASL/OAUTHBEARER token and metadata. The SASL/OAUTHBEARER token refresh callback or event handler should invoke this method upon
    ///     success. The extension keys must not include the reserved key "`auth`", and all extension keys and values must conform to the required
    ///     format as per https://tools.ietf.org/html/rfc7628#section-3.1.
    /// </summary>
    /// <param name="tokenValue">The mandatory token value to set, often (but not necessarily) a JWS compact serialization as per https://tools.ietf.org/html/rfc7515#section-3.1</param>
    /// <param name="lifetimeMs">When the token expires, in terms of the number of milliseconds since the epoch</param>
    /// <param name="principalName">The mandatory Kafka principal name associated with the token</param>
    /// <param name="extensions">Optional SASL extensions dictionary, to be communicated to the broker as additional key-value pairs during the initial client response as per https://tools.ietf.org/html/rfc7628#section-3.1</param>
    void SetToken(string tokenValue, long lifetimeMs, string principalName, IDictionary<string, string> extensions = null);

    /// <summary>
    ///     SASL/OAUTHBEARER token refresh failure indicator. The SASL/OAUTHBEARER token refresh callback or event handler should invoke this method upon failure.
    /// </summary>
    /// <param name="error">Mandatory human readable error reason for failing to acquire a token</param>
    void SetTokenFailure(string error);
}
