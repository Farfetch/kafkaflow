namespace KafkaFlow.Client.Protocol.Security
{
    using System.IO;
    using System.Threading.Tasks;

    internal class NullSecurityProtocol : ISecurityProtocol
    {
        public static readonly ISecurityProtocol Instance = new NullSecurityProtocol();

        private NullSecurityProtocol()
        {
        }

        Task<long> ISecurityProtocol.AuthenticateAsync(IInternalBrokerConnection connection) => Task.FromResult(0L);

        Stream ISecurityProtocol.CreateSecureStream(Stream inner) => inner;
    }
}
