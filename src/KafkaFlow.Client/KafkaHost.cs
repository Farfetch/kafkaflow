namespace KafkaFlow.Client
{
    using System;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Core;
    using KafkaFlow.Client.Protocol;
    using KafkaFlow.Client.Protocol.Messages;
    using KafkaFlow.Client.Protocol.Messages.Implementations;

    internal class KafkaHost : IKafkaHost
    {
        private readonly KafkaHostAddress address;
        private readonly string clientId;
        private readonly TimeSpan requestTimeout;

        private readonly AsyncLazy<IKafkaHostConnection> lazyConnection;

        public KafkaHost(KafkaHostAddress address, string clientId, TimeSpan requestTimeout)
        {
            this.address = address;
            this.clientId = clientId;
            this.requestTimeout = requestTimeout;

            this.lazyConnection = new AsyncLazy<IKafkaHostConnection>(this.CreateConnection);
        }

        public IRequestFactory RequestFactory { get; private set; } = new RequestFactory(null);

        public async Task<TResponse> SendAsync<TResponse>(IRequestMessage<TResponse> request)
            where TResponse : class, IResponse
        {
            var connection = await this.lazyConnection.Value.ConfigureAwait(false);

            return await connection.SendAsync(request).ConfigureAwait(false);
        }

        private async ValueTask<IKafkaHostConnection> CreateConnection()
        {
            var connection = new KafkaHostConnection(
                this.address.Host,
                this.address.Port,
                this.clientId,
                this.requestTimeout);

            var apiVersionResponse = await connection
                .SendAsync(new ApiVersionV2Request())
                .ConfigureAwait(false);

            if (apiVersionResponse.Error != ErrorCode.None)
            {
                throw new Exception($"Error trying to get Kafka host api version: {apiVersionResponse.Error.ToString()}");
            }

            return connection;
        }

        public void Dispose()
        {
            if (this.lazyConnection.IsValueCreated)
                this.lazyConnection.Value.Result.Dispose();
        }
    }
}
