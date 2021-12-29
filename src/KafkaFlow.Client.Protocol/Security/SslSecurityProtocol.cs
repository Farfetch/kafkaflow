namespace KafkaFlow.Client.Protocol.Security
{
    using System;
    using System.IO;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Protocol.Security.Authentication;

    public class SslSecurityProtocol : ISecurityProtocol
    {
        private readonly IAuthenticationMethod? authenticationMethod;
        private readonly X509CertificateCollection? clientCertificates;

        private SslStream stream;

        public SslSecurityProtocol(IAuthenticationMethod authenticationMethod, X509CertificateCollection clientCertificates)
        {
            this.authenticationMethod = authenticationMethod;
            this.clientCertificates = clientCertificates;
        }

        public SslSecurityProtocol(X509CertificateCollection clientCertificates)
            : this(null, clientCertificates)
        {
        }

        public SslSecurityProtocol(IAuthenticationMethod authenticationMethod)
            : this(authenticationMethod, null)
        {
        }

        async Task<long> ISecurityProtocol.AuthenticateAsync(IInternalBrokerConnection connection)
        {
            await this.stream.AuthenticateAsClientAsync(connection.Address.Host, this.clientCertificates, false);

            if (this.authenticationMethod is null)
            {
                return Convert.ToInt64((DateTime.MaxValue - DateTime.Now).TotalMilliseconds);
            }

            return await this.authenticationMethod.AuthenticateAsync(connection);
        }

        Stream ISecurityProtocol.CreateSecureStream(Stream inner)
        {
            return this.stream = new SslStream(inner, false);
        }
    }
}
