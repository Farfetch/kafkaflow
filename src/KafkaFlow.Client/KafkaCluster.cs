namespace KafkaFlow.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    internal class KafkaCluster : IKafkaCluster
    {
        private readonly IReadOnlyCollection<KafkaHostAddress> addresses;
        private readonly string clientId;
        private readonly TimeSpan requestTimeout;

        private readonly Dictionary<int, IKafkaHost> hosts = new Dictionary<int, IKafkaHost>();

        private readonly SemaphoreSlim initializeSemaphore = new SemaphoreSlim(1, 1);

        public KafkaCluster(
            IReadOnlyCollection<KafkaHostAddress> addresses,
            string clientId,
            TimeSpan requestTimeout)
        {
            this.addresses = addresses;
            this.clientId = clientId;
            this.requestTimeout = requestTimeout;
        }

        public IKafkaHost AnyHost => this.hosts.Values.First();

        public IKafkaHost GetHost(int hostId) =>
            this.hosts.TryGetValue(hostId, out var host) ?
                host :
                throw new InvalidOperationException($"There os no host with id {hostId}");

        public async ValueTask EnsureInitializationAsync()
        {
            if (this.hosts.Any())
                return;

            await this.initializeSemaphore.WaitAsync().ConfigureAwait(false);

            try
            {
                if (this.hosts.Any())
                    return;

                var firstHostAddress = this.addresses.First();
                var firstHost = new KafkaHost(firstHostAddress, this.clientId, this.requestTimeout);

                var metadata = await firstHost.Connection
                    .SendAsync(firstHost.RequestFactory.CreateMetadata())
                    .ConfigureAwait(false);

                foreach (var broker in metadata.Brokers)
                {
                    if (broker.Host == firstHostAddress.Host && broker.Port == firstHostAddress.Port)
                    {
                        this.hosts.Add(broker.NodeId, firstHost);
                    }
                    else
                    {
                        this.hosts.Add(
                            broker.NodeId,
                            new KafkaHost(
                                new KafkaHostAddress(broker.Host, broker.Port),
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

        public ValueTask<IKafkaHost> GetCoordinatorAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
