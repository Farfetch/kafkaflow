namespace KafkaFlow.Configuration
{
    /// <summary>
    /// </summary>
    public class SchemaRegistryConfiguration
    {
        /// <summary>
        ///     Specifies the configuration property(ies) that provide the basic authentication credentials.
        /// </summary>
        public AuthCredentialsSource? BasicAuthCredentialsSource { get; set; }

        /// <summary>
        ///     A comma-separated list of URLs for schema registry instances that are
        ///     used to register or lookup schemas.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        ///     Specifies the timeout for requests to Confluent Schema Registry.
        /// 
        ///     default: 30000
        /// </summary>
        public int? RequestTimeoutMs { get; set; }

        /// <summary>
        ///    File or directory path to CA certificate(s) for verifying the schema registry's key.
        /// 
        ///     default: ''
        ///     importance: low
        /// </summary>
        public string SslCaLocation { get; set; }

        /// <summary>
        ///     Path to client's keystore (PKCS#12) used for authentication.
        /// 
        ///     default: ''
        ///     importance: low
        /// </summary>
        public string SslKeystoreLocation { get; set; }

        /// <summary>
        ///     Client's keystore (PKCS#12) password.
        /// 
        ///     default: ''
        ///     importance: low
        /// </summary>
        public string SslKeystorePassword { get; set; }

        /// <summary>
        ///     Enable/Disable SSL server certificate verification. Only use in contained test/dev environments.
        /// 
        ///     default: ''
        ///     importance: low
        /// </summary>
        public bool? EnableSslCertificateVerification { get; set; }

        /// <summary>
        ///     Specifies the maximum number of schemas CachedSchemaRegistryClient
        ///     should cache locally.
        /// 
        ///     default: 1000
        /// </summary>
        public int? MaxCachedSchemas { get; set; }

        /// <summary>
        ///     Basic auth credentials in the form {username}:{password}.
        /// </summary>
        public string BasicAuthUserInfo { get; set; }
    }
}
