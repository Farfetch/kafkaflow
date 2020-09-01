namespace KafkaFlow.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Protocol;
    using KafkaFlow.Client.Protocol.Messages;

    internal class KafkaCluster : IKafkaCluster
    {
        private readonly IReadOnlyCollection<BrokerAddress> addresses;
        private readonly string clientId;
        private readonly TimeSpan requestTimeout;

        private readonly Dictionary<int, IKafkaBroker> brokers = new Dictionary<int, IKafkaBroker>();

        private readonly SemaphoreSlim initializeSemaphore = new SemaphoreSlim(1, 1);

        public KafkaCluster(
            IReadOnlyCollection<BrokerAddress> addresses,
            string clientId,
            TimeSpan requestTimeout)
        {
            this.addresses = addresses;
            this.clientId = clientId;
            this.requestTimeout = requestTimeout;
        }

        public IKafkaBroker AnyBroker => this.brokers.Values.First();

        public IKafkaBroker GetBroker(int hostId) =>
            this.brokers.TryGetValue(hostId, out var host) ?
                host :
                throw new InvalidOperationException($"There os no host with id {hostId}");

        public async ValueTask EnsureInitializationAsync()
        {
            if (this.brokers.Any())
                return;

            await this.initializeSemaphore.WaitAsync().ConfigureAwait(false);

            try
            {
                if (this.brokers.Any())
                    return;

                var first = new KafkaBroker(this.addresses.First(), 0, this.clientId, this.requestTimeout);

                var metadata = await first.Connection
                    .SendAsync(first.RequestFactory.CreateMetadata())
                    .ConfigureAwait(false);

                this.brokers.Add(first.NodeId, first);

                foreach (var broker in metadata.Brokers)
                {
                    if (broker.Host == first.Address.Host && broker.Port == first.Address.Port)
                    {
                        first.NodeId = broker.NodeId;
                        this.brokers.Add(first.NodeId, first);
                    }
                    else
                    {
                        this.brokers.Add(
                            broker.NodeId,
                            new KafkaBroker(
                                new BrokerAddress(broker.Host, broker.Port),
                                broker.NodeId,
                                this.clientId,
                                this.requestTimeout));
                    }
                }
            }
            finally
            {
                this.initializeSemaphore.Release();
            }
        }

        public ValueTask<IKafkaBroker> GetCoordinatorAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
