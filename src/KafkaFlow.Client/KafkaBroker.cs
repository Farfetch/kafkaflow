namespace KafkaFlow.Client
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Protocol;
    using KafkaFlow.Client.Protocol.Messages;
    using KafkaFlow.Client.Protocol.Messages.Implementations;

    internal class KafkaBroker : IKafkaBroker
    {
        private readonly Lazy<Task<IRequestFactory>> lazyRequestFactory;

        public KafkaBroker(BrokerAddress address, int nodeId, string clientId, TimeSpan requestTimeout)
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

        private async Task<IRequestFactory> CreateRequestFactoryAsync()
        {
            var apiVersionResponse = await this.Connection.SendAsync(new ApiVersionV2Request());

            if (apiVersionResponse.Error != ErrorCode.None)
            {
                throw new Exception($"Error trying to get Kafka host api version: {apiVersionResponse.Error}");
            }

            return new RequestFactory(
                new BrokerCapabilities(
                    apiVersionResponse.ApiVersions
                        .Select(x => new ApiVersionRange(x.ApiKey, x.MinVersion, x.MaxVersion))));
        }

        public async ValueTask DisposeAsync()
        {
            await this.Connection.DisposeAsync();
        }
    }
}
