namespace KafkaFlow.Client
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Exceptions;
    using KafkaFlow.Client.Protocol;
    using KafkaFlow.Client.Protocol.Messages;
    using KafkaFlow.Client.Protocol.Security;

    internal class KafkaBroker : IKafkaBroker
    {
        private readonly Lazy<IRequestFactory> lazyRequestFactory;

        public KafkaBroker(
            BrokerAddress address,
            string clientId,
            TimeSpan requestTimeout,
            ISecurityProtocol securityProtocol)
        {
            this.Connection = new BrokerConnection(
                address,
                clientId,
                requestTimeout,
                securityProtocol);

            this.lazyRequestFactory = new Lazy<IRequestFactory>(this.CreateRequestFactory);
        }

        public IBrokerConnection Connection { get; }

        public IRequestFactory RequestFactory => this.lazyRequestFactory.Value;

        public async ValueTask DisposeAsync()
        {
            await this.Connection.DisposeAsync();
        }

        private IRequestFactory CreateRequestFactory()
        {
            var factory = new RequestFactory();

            var apiVersion = this.Connection.SendAsync(factory.CreateApiVersion()).GetAwaiter().GetResult();

            if (apiVersion.Error != ErrorCode.None)
            {
                throw new BrokerApiVersionException(apiVersion.Error, "Error trying to get Kafka host api version.");
            }

            factory.SetBrokerCapabilities(
                new BrokerCapabilities(
                    apiVersion.ApiVersions
                        .Select(x => new ApiVersionRange(x.ApiKey, x.MinVersion, x.MaxVersion))));

            return factory;
        }
    }
}
