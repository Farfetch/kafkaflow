namespace KafkaFlow.Configuration
{
    /// <summary>AuthCredentialsSource enum values</summary>
    public enum AuthCredentialsSource
    {
        /// <summary>
        ///     Credentials are specified via the `schema.registry.basic.auth.user.info` config property in the form username:password.
        ///     If `schema.registry.basic.auth.user.info` is not set, authentication is disabled.
        /// </summary>
        UserInfo,
        /// <summary>
        ///     Credentials are specified via the `sasl.username` and `sasl.password` configuration properties.
        /// </summary>
        SaslInherit,
    }
}