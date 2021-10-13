namespace KafkaFlow.Client
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Protocol;

    internal class KafkaCluster : IKafkaCluster
    {
        private readonly IReadOnlyCollection<BrokerAddress> addresses;
        private readonly string clientId;
        private readonly TimeSpan requestTimeout;
        private readonly ConcurrentDictionary<int, IKafkaBroker> brokers = new();
        private readonly SemaphoreSlim semaphore = new(1, 1);

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

        public async ValueTask EnsureInitializationAsync()
        {
            if (!this.brokers.IsEmpty)
            {
                return;
            }

            await this.semaphore.WaitAsync().ConfigureAwait(false);

            try
            {
                if (!this.brokers.IsEmpty)
                {
                    return;
                }

                var firstBroker = new KafkaBroker(this.addresses.First(), 0, this.clientId, this.requestTimeout);
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
                            this.requestTimeout));
                }
            }
            finally
            {
                this.semaphore.Release();
            }
        }
    }
}
