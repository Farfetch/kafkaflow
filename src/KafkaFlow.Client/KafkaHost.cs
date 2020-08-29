namespace KafkaFlow.Client
{
    using System;
    using System.Linq;
    using KafkaFlow.Client.Protocol;
    using KafkaFlow.Client.Protocol.Messages;
    using KafkaFlow.Client.Protocol.Messages.Implementations;

    internal class KafkaHost : IKafkaHost
    {
        private readonly Lazy<IRequestFactory> lazyRequestFactory;

        public KafkaHost(KafkaHostAddress address, string clientId, TimeSpan requestTimeout)
        {
            this.Connection = new KafkaHostConnection(
                address.Host,
                address.Port,
                clientId,
                requestTimeout);

            this.lazyRequestFactory = new Lazy<IRequestFactory>(this.CreateRequestFactory);
        }

        public IKafkaHostConnection Connection { get; }

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
                new HostCapabilities(
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
