namespace KafkaFlow.Client
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Exceptions;
    using KafkaFlow.Client.Protocol;
    using KafkaFlow.Client.Protocol.Messages;
    using KafkaFlow.Client.Protocol.Messages.Implementations.ApiVersion;

    internal class KafkaBroker : IKafkaBroker
    {
        private readonly Lazy<Task<IRequestFactory>> lazyRequestFactory;

        public KafkaBroker(BrokerAddress address, int nodeId, string clientId, TimeSpan? requestTimeout)
        {
            this.Address = address;
            this.NodeId = nodeId;
            this.Connection = new BrokerConnection(
                address,
                clientId,
                requestTimeout);

            this.lazyRequestFactory = new Lazy<Task<IRequestFactory>>(this.CreateRequestFactoryAsync);
        }

        public BrokerAddress Address { get; }

        public int NodeId { get; set; }

        public IBrokerConnection Connection { get; }

        public Task<IRequestFactory> GetRequestFactoryAsync() => this.lazyRequestFactory.Value;

        public async ValueTask DisposeAsync()
        {
            await this.Connection.DisposeAsync();
        }

        private async Task<IRequestFactory> CreateRequestFactoryAsync()
        {
            var apiVersion = await this.Connection.SendAsync(new ApiVersionV2Request()).ConfigureAwait(false);

            if (apiVersion.Error != ErrorCode.None)
            {
                throw new BrokerApiVersionException(apiVersion.Error, "Error trying to get Kafka host api version.");
            }

            return new RequestFactory(
                new BrokerCapabilities(
                    apiVersion.ApiVersions
                        .Select(x => new ApiVersionRange(x.ApiKey, x.MinVersion, x.MaxVersion))));
        }
    }
}
