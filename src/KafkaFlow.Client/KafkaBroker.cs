namespace KafkaFlow.Client
{
    using System;
    using System.Linq;
    using KafkaFlow.Client.Protocol;
    using KafkaFlow.Client.Protocol.Messages;
    using KafkaFlow.Client.Protocol.Messages.Implementations;

    internal class KafkaBroker : IKafkaBroker
    {
        private readonly Lazy<IRequestFactory> lazyRequestFactory;

        public KafkaBroker(BrokerAddress address, string clientId, TimeSpan requestTimeout)
        {
            this.Connection = new BrokerConnection(
                address.Host,
                address.Port,
                clientId,
                requestTimeout);

            this.lazyRequestFactory = new Lazy<IRequestFactory>(this.CreateRequestFactory);
        }

        public IBrokerConnection Connection { get; }

        private IRequestFactory CreateRequestFactory()
        {
            var apiVersionResponse = this.Connection
                .SendAsync(new ApiVersionV2Request())
                .GetAwaiter()
                .GetResult();

            if (apiVersionResponse.Error != ErrorCode.None)
            {
                throw new Exception($"Error trying to get Kafka host api version: {apiVersionResponse.Error}");
            }

            return new RequestFactory(
                new BrokerCapabilities(
                    apiVersionResponse.ApiVersions
                        .Select(x => new ApiVersionRange(x.ApiKey, x.MinVersion, x.MaxVersion))));
        }

        public IRequestFactory RequestFactory => this.lazyRequestFactory.Value;

        public void Dispose()
        {
            this.Connection.Dispose();
        }
    }
}
