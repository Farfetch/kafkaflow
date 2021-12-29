namespace KafkaFlow.Client.Protocol.Security
{
    using System.IO;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Protocol.Security.Authentication;

    public class PlaintextSecurityProtocol : ISecurityProtocol
    {
        private readonly IAuthenticationMethod authenticationMethod;

        public PlaintextSecurityProtocol(IAuthenticationMethod authenticationMethod)
        {
            this.authenticationMethod = authenticationMethod;
        }

        async Task<long> ISecurityProtocol.AuthenticateAsync(IInternalBrokerConnection connection)
        {
            return await this.authenticationMethod.AuthenticateAsync(connection);
        }

        Stream ISecurityProtocol.CreateSecureStream(Stream inner) => inner;
    }
}
