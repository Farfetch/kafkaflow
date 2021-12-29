namespace KafkaFlow.Client.Protocol.Security.Authentication
{
    using System.Threading.Tasks;

    public interface IAuthenticationMethod
    {
        internal Task<long> AuthenticateAsync(IInternalBrokerConnection connection);
    }
}
