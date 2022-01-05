namespace KafkaFlow.Client.Protocol.Security
{
    using System.IO;
    using System.Threading.Tasks;

    public interface ISecurityProtocol
    {
        internal Task<long> AuthenticateAsync(IInternalBrokerConnection connection);

        internal Stream CreateSecureStream(Stream inner);
    }
}
