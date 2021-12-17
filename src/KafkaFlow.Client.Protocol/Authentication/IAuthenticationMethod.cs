namespace KafkaFlow.Client.Protocol.Authentication
{
    using System.Threading.Tasks;

    public interface IAuthenticationMethod
    {
        Task<long> AuthenticateAsync(IBrokerConnection connection);
    }
}
