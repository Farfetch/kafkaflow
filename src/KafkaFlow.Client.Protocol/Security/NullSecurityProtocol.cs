namespace KafkaFlow.Client.Protocol.Security
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class NullSecurityProtocol : ISecurityProtocol
    {
        public static readonly ISecurityProtocol Instance = new NullSecurityProtocol();

        private NullSecurityProtocol()
        {
        }

        Task<long> ISecurityProtocol.AuthenticateAsync(IInternalBrokerConnection connection)
        {
            return Task.FromResult(Convert.ToInt64((DateTime.MaxValue - DateTime.Now).TotalMilliseconds));
        }

        Stream ISecurityProtocol.CreateSecureStream(Stream inner) => inner;
    }
}
