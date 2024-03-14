using System;
using KafkaFlow.Authentication;

namespace KafkaFlow.Configuration;

/// <summary>
/// Represent the Kafka security information
/// </summary>
public class SecurityInformation
{
    /// <summary>
    ///     Gets or sets the protocol used to communicate with brokers.
    ///
    ///     default: plaintext
    ///     importance: high
    /// </summary>
    public SecurityProtocol? SecurityProtocol { get; set; }

    /// <summary>
    ///     Gets or sets the SASL mechanism to use for authentication. Supported: GSSAPI, PLAIN, SCRAM-SHA-256, SCRAM-SHA-512. **NOTE**: Despite the name, you may not configure more than one mechanism.
    /// </summary>
    public SaslMechanism? SaslMechanism { get; set; }

    /// <summary>
    ///     Gets or sets the cipher suites. A cipher suite is a named combination of authentication, encryption, MAC and key exchange algorithm used to negotiate the security settings for a network connection using TLS or SSL network protocol. See manual page for `ciphers(1)` and `SSL_CTX_set_cipher_list(3).
    ///
    ///     default: ''
    ///     importance: low
    /// </summary>
    public string SslCipherSuites { get; set; }

    /// <summary>
    ///     Gets or sets the supported-curves extension in the TLS ClientHello message specifies the curves (standard/named, or 'explicit' GF(2^k) or GF(p)) the client is willing to have the server use. See manual page for `SSL_CTX_set1_curves_list(3)`. OpenSSL &gt;= 1.0.2 required.
    ///
    ///     default: ''
    ///     importance: low
    /// </summary>
    public string SslCurvesList { get; set; }

    /// <summary>
    ///     Gets or sets the sigalgs list. The client uses the TLS ClientHello signature_algorithms extension to indicate to the server which signature/hash algorithm pairs may be used in digital signatures. See manual page for `SSL_CTX_set1_sigalgs_list(3)`. OpenSSL &gt;= 1.0.2 required.
    ///
    ///     default: ''
    ///     importance: low
    /// </summary>
    public string SslSigalgsList { get; set; }

    /// <summary>
    ///     Gets or sets the path to client's private key (PEM) used for authentication.
    ///
    ///     default: ''
    ///     importance: low
    /// </summary>
    public string SslKeyLocation { get; set; }

    /// <summary>
    ///     Gets or sets the private key passphrase (for use with `ssl.key.location` and `set_ssl_cert()`)
    ///
    ///     default: ''
    ///     importance: low
    /// </summary>
    public string SslKeyPassword { get; set; }

    /// <summary>
    ///     Gets or sets the client's private key string (PEM format) used for authentication.
    ///
    ///     default: ''
    ///     importance: low
    /// </summary>
    public string SslKeyPem { get; set; }

    /// <summary>
    ///     Gets or sets the path to client's public key (PEM) used for authentication.
    ///
    ///     default: ''
    ///     importance: low
    /// </summary>
    public string SslCertificateLocation { get; set; }

    /// <summary>
    ///     Gets or sets client's public key string (PEM format) used for authentication.
    ///
    ///     default: ''
    ///     importance: low
    /// </summary>
    public string SslCertificatePem { get; set; }

    /// <summary>
    ///     Gets or sets file or directory path to CA certificate(s) for verifying the broker's key.
    ///
    ///     default: ''
    ///     importance: low
    /// </summary>
    public string SslCaLocation { get; set; }

    /// <summary>
    ///     Gets or sets the CA certificate(s) value for verifying the broker's key.
    ///
    ///     default: ''
    ///     importance: low
    /// </summary>
    public string SslCaPem { get; set; }

    /// <summary>
    ///     Gets or sets the path to CRL for verifying broker's certificate validity.
    ///
    ///     default: ''
    ///     importance: low
    /// </summary>
    public string SslCrlLocation { get; set; }

    /// <summary>
    ///     Gets or sets the path to client's keystore (PKCS#12) used for authentication.
    ///
    ///     default: ''
    ///     importance: low
    /// </summary>
    public string SslKeystoreLocation { get; set; }

    /// <summary>
    ///     Gets or sets the client's keystore (PKCS#12) password.
    ///
    ///     default: ''
    ///     importance: low
    /// </summary>
    public string SslKeystorePassword { get; set; }

    /// <summary>
    ///     Gets or sets the Enable OpenSSL's builtin broker (server) certificate verification. This verification can be extended by the application by implementing a certificate_verify_cb.
    ///
    ///     default: true
    ///     importance: low
    /// </summary>
    public bool? EnableSslCertificateVerification { get; set; }

    /// <summary>
    ///     Gets or sets the endpoint identification algorithm to validate broker hostname using broker certificate. https - Server (broker) hostname verification as specified in RFC2818. none - No endpoint verification. OpenSSL &gt;= 1.0.2 required.
    ///
    ///     default: none
    ///     importance: low
    /// </summary>
    public SslEndpointIdentificationAlgorithm? SslEndpointIdentificationAlgorithm { get; set; }

    /// <summary>
    ///     Gets or sets the Kerberos principal name that Kafka runs as, not including /hostname@REALM
    ///
    ///     default: kafka
    ///     importance: low
    /// </summary>
    public string SaslKerberosServiceName { get; set; }

    /// <summary>
    ///     Gets or sets the client's Kerberos principal name. (Not supported on Windows, will use the logon user's principal).
    ///
    ///     default: kafkaclient
    ///     importance: low
    /// </summary>
    public string SaslKerberosPrincipal { get; set; }

    /// <summary>
    ///     Gets or sets the shell command to refresh or acquire the client's Kerberos ticket. This command is executed on client creation and every sasl.kerberos.min.time.before.relogin (0=disable). %{config.prop.name} is replaced by corresponding config object value.
    ///
    ///     default: kinit -R -t "%{sasl.kerberos.keytab}" -k %{sasl.kerberos.principal} || kinit -t "%{sasl.kerberos.keytab}" -k %{sasl.kerberos.principal}
    ///     importance: low
    /// </summary>
    public string SaslKerberosKinitCmd { get; set; }

    /// <summary>
    ///     Gets or sets the path to Kerberos keytab file. This configuration property is only used as a variable in `sasl.kerberos.kinit.cmd` as ` ... -t "%{sasl.kerberos.keytab}"`.
    ///
    ///     default: ''
    ///     importance: low
    /// </summary>
    public string SaslKerberosKeytab { get; set; }

    /// <summary>
    ///     Gets or sets the minimum time in milliseconds between key refresh attempts. Disable automatic key refresh by setting this property to 0.
    ///
    ///     default: 60000
    ///     importance: low
    /// </summary>
    public int? SaslKerberosMinTimeBeforeRelogin { get; set; }

    /// <summary>
    ///     Gets or sets the SASL username for use with the PLAIN and SASL-SCRAM-.. mechanisms
    ///
    ///     default: ''
    ///     importance: high
    /// </summary>
    public string SaslUsername { get; set; }

    /// <summary>
    ///     Gets or sets the SASL password for use with the PLAIN and SASL-SCRAM-.. mechanism
    ///
    ///     default: ''
    ///     importance: high
    /// </summary>
    public string SaslPassword { get; set; }

    /// <summary>
    ///     Gets or sets the SASL/OAUTHBEARER configuration. The format is implementation-dependent and must be parsed accordingly. The default unsecured token implementation (see https://tools.ietf.org/html/rfc7515#appendix-A.5) recognizes space-separated name=value pairs with valid names including principalClaimName, principal, scopeClaimName, scope, and lifeSeconds. The default value for principalClaimName is "sub", the default value for scopeClaimName is "scope", and the default value for lifeSeconds is 3600. The scope value is CSV format with the default value being no/empty scope. For example: `principalClaimName=azp principal=admin scopeClaimName=roles scope=role1,role2 lifeSeconds=600`. In addition, SASL extensions can be communicated to the broker via `extension_NAME=value`. For example: `principal=admin extension_traceId=123`
    ///
    ///     default: ''
    ///     importance: low
    /// </summary>
    public string SaslOauthbearerConfig { get; set; }

    /// <summary>
    ///     Gets or sets enable the builtin unsecure JWT OAUTHBEARER token handler if no oauthbearer_refresh_cb has been set. This builtin handler should only be used for development or testing, and not in production.
    ///
    ///     default: false
    ///     importance: low
    /// </summary>
    public bool? EnableSaslOauthbearerUnsecureJwt { get; set; }

    /// <summary>
    ///     Gets or sets the SaslOauthbearerMethod
    ///
    ///     default: false
    ///     importance: low
    /// </summary>
    public SaslOauthbearerMethod? SaslOauthbearerMethod { get; set; }

    /// <summary>
    ///     Gets or sets the SaslOauthbearerClientId
    ///
    ///     default: false
    ///     importance: low
    /// </summary>
    public string SaslOauthbearerClientId { get; set; }

    /// <summary>
    ///     Gets or sets the SaslOauthbearerClientSecret
    ///
    ///     default: false
    ///     importance: low
    /// </summary>
    public string SaslOauthbearerClientSecret { get; set; }

    /// <summary>
    ///     Gets or sets the SaslOauthbearerTokenEndpointUrl
    ///
    ///     default: false
    ///     importance: low
    /// </summary>
    public string SaslOauthbearerTokenEndpointUrl { get; set; }

    /// <summary>
    ///     Gets or sets the SaslOauthbearerScope
    ///
    ///     default: false
    ///     importance: low
    /// </summary>
    public string SaslOauthbearerScope { get; set; }

    /// <summary>
    ///     Gets or sets the OAuthBearerTokenRefreshHandler for custom OAuth authentication.
    /// </summary>
    public Action<IOAuthBearerAuthenticator> OAuthBearerTokenRefreshHandler { get; set; }
}
