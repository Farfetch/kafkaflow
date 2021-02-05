namespace KafkaFlow.Serializer.ApacheAvro
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Configuration;
    using Confluent.SchemaRegistry;
    using AuthCredentialsSource = Configuration.AuthCredentialsSource;

    internal static class ConfigurationExtensions
    {
        public static SchemaRegistryConfig ToConfluent(this SchemaRegistryConfiguration configuration)
        {
            var config = new SchemaRegistryConfig();
            config
                .SetIfNotNull(c => c.Url, configuration.Url)
                .SetIfNotNull(c => c.BasicAuthCredentialsSource, configuration.BasicAuthCredentialsSource?.ToConfluent())
                .SetIfNotNull(c => c.RequestTimeoutMs, configuration.RequestTimeoutMs)
                .SetIfNotNull(c => c.SslCaLocation, configuration.SslCaLocation)
                .SetIfNotNull(c => c.SslKeystoreLocation, configuration.SslKeystoreLocation)
                .SetIfNotNull(c => c.SslKeystorePassword, configuration.SslKeystorePassword)
                .SetIfNotNull(c => c.EnableSslCertificateVerification, configuration.EnableSslCertificateVerification)
                .SetIfNotNull(c => c.BasicAuthUserInfo, configuration.BasicAuthUserInfo)
                .SetIfNotNull(c => c.MaxCachedSchemas, configuration.MaxCachedSchemas);
            
            return config;
        }

        private static Confluent.SchemaRegistry.AuthCredentialsSource ToConfluent(this AuthCredentialsSource authCredentialsSource) => authCredentialsSource switch
        {
            AuthCredentialsSource.SaslInherit => Confluent.SchemaRegistry.AuthCredentialsSource.SaslInherit,
            AuthCredentialsSource.UserInfo => Confluent.SchemaRegistry.AuthCredentialsSource.UserInfo,
            _ => throw new ArgumentOutOfRangeException()
        };

        private static SchemaRegistryConfig SetIfNotNull<TProperty>
        (
            this SchemaRegistryConfig arg,
            Expression<Func<SchemaRegistryConfig, TProperty>> propertyPicker,
            object value
        )
        {
            if (value == null)
            {
                return arg;
            }
            
            var prop = (PropertyInfo)((MemberExpression)propertyPicker.Body).Member;
            prop.SetValue(arg, value, null);
            return arg;
        }
    }
}
