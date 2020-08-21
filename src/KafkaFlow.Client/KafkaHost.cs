namespace KafkaFlow.Client
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Core;
    using KafkaFlow.Client.Messages;
    using KafkaFlow.Client.Messages.Adapters;
    using KafkaFlow.Client.Protocol;
    using KafkaFlow.Client.Protocol.Messages;

    internal class KafkaHost : IKafkaHost
    {
        private readonly KafkaHostAddress address;
        private readonly string clientId;
        private readonly TimeSpan requestTimeout;

        private readonly AsyncLazy<IHostConnectionProxy> lazyConnection;

        public KafkaHost(KafkaHostAddress address, string clientId, TimeSpan requestTimeout)
        {
            this.address = address;
            this.clientId = clientId;
            this.requestTimeout = requestTimeout;

            this.lazyConnection = new AsyncLazy<IHostConnectionProxy>(this.CreateConnection);
        }

        public async Task<TResponse> SendAsync<TResponse>(IClientRequest<TResponse> request)
            where TResponse : IClientResponse
        {
            var connection = await this.lazyConnection.Value.ConfigureAwait(false);

            return await connection.SendAsync(request).ConfigureAwait(false);
        }

        private async ValueTask<IHostConnectionProxy> CreateConnection()
        {
            var rawConnection = new KafkaHostConnection(
                this.address.Host,
                this.address.Port,
                this.clientId,
                this.requestTimeout);

            var apiVersionResponse = await rawConnection
                .SendAsync(new ApiVersionV2Request())
                .ConfigureAwait(false);

            if (apiVersionResponse.Error != ErrorCode.None)
            {
                throw new Exception($"Error trying to get Kafka host api version: {apiVersionResponse.Error.ToString()}");
            }

            var hostCapabilities = new HostCapabilities(
                apiVersionResponse.ApiVersions
                    .Select(x => new ApiVersionRange(x.ApiKey, x.MinVersion, x.MaxVersion)));

            return new HostConnectionProxy(rawConnection, hostCapabilities);
        }

        public void Dispose()
        {
            if (this.lazyConnection.IsValueCreated)
                this.lazyConnection.Value.Result.Dispose();
        }
    }
}
