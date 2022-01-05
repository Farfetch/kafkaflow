namespace KafkaFlow.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Metadata;
    using KafkaFlow.Client.Protocol;
    using KafkaFlow.Client.Protocol.Messages;
    using KafkaFlow.Client.Protocol.Security;

    public class KafkaCluster : IKafkaCluster
    {
        private readonly Dictionary<int, IKafkaBroker> brokers = new();
        private readonly SemaphoreSlim initializeSemaphore = new(1, 1);

        private readonly IReadOnlyCollection<BrokerAddress> addresses;
        private readonly string clientId;
        private readonly TimeSpan requestTimeout;
        private readonly ISecurityProtocol securityProtocol;
        private readonly Lazy<IMetadataClient> metadataClient;

        public KafkaCluster(
            IReadOnlyCollection<BrokerAddress> addresses,
            string clientId,
            TimeSpan? requestTimeout = null,
            ISecurityProtocol? securityProtocol = null)
        {
            this.addresses = addresses;
            this.clientId = clientId;
            this.requestTimeout = requestTimeout ?? TimeSpan.FromSeconds(30);
            this.securityProtocol = securityProtocol ?? NullSecurityProtocol.Instance;
            this.metadataClient = new Lazy<IMetadataClient>(() => new MetadataClient(this));
        }

        public IReadOnlyDictionary<int, IKafkaBroker> Brokers
        {
            get
            {
                this.EnsureInitialization();
                return this.brokers;
            }
        }

        public IMetadataClient MetadataClient => this.metadataClient.Value;

        /// <inheritdoc />>
        public async ValueTask DisposeAsync()
        {
            this.initializeSemaphore.Dispose();
            await Task.WhenAll(this.brokers.Select(b => b.Value.DisposeAsync().AsTask()));
        }

        private void EnsureInitialization()
        {
            if (this.brokers.Any())
            {
                return;
            }

            lock (this.brokers)
            {
                if (this.brokers.Any())
                {
                    return;
                }

                var metadata = this.GetClusterMetadataAsync().GetAwaiter().GetResult();

                foreach (var broker in metadata.Brokers)
                {
                    this.brokers.Add(
                        broker.NodeId,
                        new KafkaBroker(
                            new BrokerAddress(broker.Host, broker.Port),
                            this.clientId,
                            this.requestTimeout,
                            this.securityProtocol));
                }
            }
        }

        private async Task<IMetadataResponse> GetClusterMetadataAsync()
        {
            await using var broker = new KafkaBroker(
                this.addresses.First(),
                this.clientId,
                this.requestTimeout,
                this.securityProtocol);

            var metadata = await broker
                .Connection
                .SendAsync(broker.RequestFactory.CreateMetadata())
                .ConfigureAwait(false);

            return metadata;
        }
    }
}
