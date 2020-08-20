namespace KafkaFlow.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Protocol;
    using KafkaFlow.Client.Protocol.Security;

    public class KafkaCluster : IKafkaCluster
    {
        private readonly IReadOnlyCollection<BrokerAddress> addresses;
        private readonly string clientId;
        private readonly TimeSpan requestTimeout;
        private readonly ISecurityProtocol securityProtocol;

        private readonly Dictionary<int, IKafkaBroker> brokers = new();

        private readonly SemaphoreSlim initializeSemaphore = new(1, 1);

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
        }

        public IKafkaBroker AnyBroker => this.brokers.Values.First();

        public IKafkaBroker GetBroker(int hostId) =>
            this.brokers.TryGetValue(hostId, out var host) ?
                host :
                throw new InvalidOperationException($"There os no host with id {hostId}");

        public async ValueTask EnsureInitializationAsync()
        {
            if (this.brokers.Any())
            {
                return;
            }

            await this.initializeSemaphore.WaitAsync().ConfigureAwait(false);

            try
            {
                if (this.brokers.Any())
                {
                    return;
                }

                var firstBroker = new KafkaBroker(
                    this.addresses.First(),
                    0,
                    this.clientId,
                    this.requestTimeout,
                    this.securityProtocol);

                this.brokers.TryAdd(firstBroker.NodeId, firstBroker);

                var requestFactory = await firstBroker.GetRequestFactoryAsync();

                var metadata = await firstBroker
                    .Connection
                    .SendAsync(requestFactory.CreateMetadata())
                    .ConfigureAwait(false);

                firstBroker.NodeId = metadata
                    .Brokers
                    .First(b => b.Host == firstBroker.Address.Host && b.Port == firstBroker.Address.Port)
                    .NodeId;

                foreach (var broker in metadata.Brokers)
                {
                    this.brokers.TryAdd(
                        broker.NodeId,
                        new KafkaBroker(
                            new BrokerAddress(broker.Host, broker.Port),
                            broker.NodeId,
                            this.clientId,
                            this.requestTimeout,
                            this.securityProtocol));
                }
            }
            finally
            {
                this.initializeSemaphore.Release();
            }
        }

        public async ValueTask DisposeAsync()
        {
            await Task.WhenAll(this.brokers.Select(b => b.Value.DisposeAsync().AsTask()));
        }
    }
}
