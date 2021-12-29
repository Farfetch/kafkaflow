namespace KafkaFlow.Client
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Protocol;
    using KafkaFlow.Client.Protocol.Messages;
    using KafkaFlow.Client.Protocol.Security;

    internal class KafkaBroker : IKafkaBroker
    {
        private readonly Lazy<Task<IRequestFactory>> lazyRequestFactory;

        public KafkaBroker(
            BrokerAddress address,
            int nodeId,
            string clientId,
            TimeSpan requestTimeout,
            ISecurityProtocol securityProtocol)
        {
            this.Address = address;
            this.NodeId = nodeId;
            this.Connection = new BrokerConnection(
                address,
                clientId,
                requestTimeout,
                securityProtocol);

            this.lazyRequestFactory = new Lazy<Task<IRequestFactory>>(this.CreateRequestFactoryAsync);
        }

        public BrokerAddress Address { get; }

        public int NodeId { get; set; }

        public IBrokerConnection Connection { get; }

        public Task<IRequestFactory> GetRequestFactoryAsync() => this.lazyRequestFactory.Value;

        private async Task<IRequestFactory> CreateRequestFactoryAsync()
        {
            var factory = new RequestFactory();

            var apiVersionResponse = await this.Connection.SendAsync(factory.CreateApiVersion());

            if (apiVersionResponse.Error != ErrorCode.None)
            {
                throw new Exception($"Error trying to get Kafka host api version: {apiVersionResponse.Error}");
            }

            factory.SetBrokerCapabilities(
                new BrokerCapabilities(
                    apiVersionResponse.ApiVersions
                        .Select(x => new ApiVersionRange(x.ApiKey, x.MinVersion, x.MaxVersion))));

            return factory;
        }

        public async ValueTask DisposeAsync()
        {
            await this.Connection.DisposeAsync();
        }
    }
}
