namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using KafkaFlow.Client;
    using KafkaFlow.Client.Protocol;
    using KafkaFlow.Client.Protocol.Security;
    using KafkaFlow.Client.Protocol.Security.Authentication;
    using KafkaFlow.Client.Protocol.Security.Authentication.SASL.Scram;
    using KafkaFlow.Consumers;
    using KafkaFlow.Producers;

    internal class KafkaConfigurationBuilder : IKafkaConfigurationBuilder
    {
        private readonly IDependencyConfigurator dependencyConfigurator;
        private readonly List<ClusterConfigurationBuilder> clusters = new();
        private Type logHandlerType = typeof(NullLogHandler);

        public KafkaConfigurationBuilder(IDependencyConfigurator dependencyConfigurator)
        {
            this.dependencyConfigurator = dependencyConfigurator;
        }

        public KafkaConfiguration Build()
        {
            var configuration = new KafkaConfiguration();

            configuration.AddClusters(this.clusters.Select(x => x.Build(configuration)));

            this.dependencyConfigurator.AddSingleton<IProducerAccessor>(
                resolver => new ProducerAccessor(
                    configuration.Clusters
                        .SelectMany(x => x.Producers)
                        .Select(
                            producer => new MessageProducer(
                                resolver,
                                producer))));

            IClusterAccessor clusterAccessor = new ClusterAccessor();

            foreach (var cluster in configuration.Clusters)
            {
                try
                {
                    clusterAccessor.Add(cluster.Name, CreateCluster(cluster));
                }
                catch (NotSupportedException)
                {
                    // Do nothing
                }
            }

            this.dependencyConfigurator
                .AddTransient(typeof(ILogHandler), this.logHandlerType)
                .AddSingleton<IDateTimeProvider, DateTimeProvider>()
                .AddSingleton(clusterAccessor)
                .AddSingleton<IConsumerAccessor>(new ConsumerAccessor())
                .AddSingleton<IConsumerManagerFactory>(new ConsumerManagerFactory());

            return configuration;
        }

        public IKafkaConfigurationBuilder AddCluster(Action<IClusterConfigurationBuilder> cluster)
        {
            var builder = new ClusterConfigurationBuilder(this.dependencyConfigurator);

            cluster(builder);

            this.clusters.Add(builder);

            return this;
        }

        public IKafkaConfigurationBuilder UseLogHandler<TLogHandler>()
            where TLogHandler : ILogHandler
        {
            this.logHandlerType = typeof(TLogHandler);
            return this;
        }

        private static IKafkaCluster CreateCluster(ClusterConfiguration cluster)
        {
            return new KafkaCluster(
                cluster.Brokers
                    .Select(BrokerAddress.Parse)
                    .ToList(),
                string.Empty,
                securityProtocol: GetClusterSecurityProtocol(cluster));
        }

        private static ISecurityProtocol GetClusterSecurityProtocol(ClusterConfiguration cluster)
        {
            var securityProtocol = NullSecurityProtocol.Instance;

            var securityInformation = cluster.GetSecurityInformation();

            if (securityInformation is null)
            {
                return securityProtocol;
            }

            IAuthenticationMethod saslMechanism = securityInformation.SaslMechanism switch
            {
                SaslMechanism.ScramSha512 => new ScramSha512Authentication(
                    securityInformation.SaslUsername,
                    securityInformation.SaslPassword),
                SaslMechanism.ScramSha256 => new ScramSha256Authentication(
                    securityInformation.SaslUsername,
                    securityInformation.SaslPassword),
                _ => throw new NotSupportedException()
            };

            var sslCertificate = string.IsNullOrEmpty(securityInformation.SslCaLocation) ?
                null :
                new X509CertificateCollection(
                    new[] { X509Certificate.CreateFromSignedFile(securityInformation.SslCaLocation) });

            securityProtocol = securityInformation.SecurityProtocol switch
            {
                SecurityProtocol.SaslPlaintext => new PlaintextSecurityProtocol(saslMechanism),
                SecurityProtocol.Ssl => new SslSecurityProtocol(clientCertificates: sslCertificate),
                SecurityProtocol.SaslSsl => new SslSecurityProtocol(saslMechanism, sslCertificate),
                _ => NullSecurityProtocol.Instance
            };

            return securityProtocol;
        }
    }
}
