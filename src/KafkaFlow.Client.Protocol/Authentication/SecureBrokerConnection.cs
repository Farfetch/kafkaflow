namespace KafkaFlow.Client.Protocol.Authentication
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class SecureBrokerConnection : IBrokerConnection
    {
        private readonly SemaphoreSlim authenticationSemaphore = new(1, 1);
        private readonly IBrokerConnection connection;
        private readonly IAuthenticationMethod authentication;

        private DateTime sessionExpiryTime = DateTime.MinValue;

        public SecureBrokerConnection(
            IBrokerConnection connection,
            IAuthenticationMethod authentication)
        {
            this.connection = connection;
            this.authentication = authentication;
        }

        public async ValueTask DisposeAsync() => await this.connection.DisposeAsync();

        public async Task<TResponse> SendAsync<TResponse>(IRequestMessage<TResponse> request)
            where TResponse : class, IResponse
        {
            if (this.sessionExpiryTime >= DateTime.Now)
            {
                return await this.connection.SendAsync(request);
            }

            await this.authenticationSemaphore.WaitAsync();

            try
            {
                if (this.sessionExpiryTime < DateTime.Now)
                {
                    var sessionLifetime = await this.authentication.AuthenticateAsync(this.connection);
                    this.sessionExpiryTime = sessionLifetime == 0 ?
                        DateTime.MaxValue :
                        DateTime.Now.AddMilliseconds(sessionLifetime);
                }
            }
            finally
            {
                this.authenticationSemaphore.Release();
            }

            return await this.connection.SendAsync(request);
        }
    }
}
