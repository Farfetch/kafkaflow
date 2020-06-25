namespace KafkaFlow.Configuration
{
    using System.Collections.Generic;
    using System.Linq;

    internal class ClusterConfiguration
    {
        private readonly List<ProducerConfiguration> producers = new List<ProducerConfiguration>();
        private readonly List<ConsumerConfiguration> consumers = new List<ConsumerConfiguration>();

        public ClusterConfiguration(
            KafkaConfiguration kafka,
            IEnumerable<string> brokers)
        {
            this.Kafka = kafka;
            this.Brokers = brokers.ToList();
        }

        public KafkaConfiguration Kafka { get; }

        public IReadOnlyCollection<string> Brokers { get; }

        public IReadOnlyCollection<ProducerConfiguration> Producers => this.producers.AsReadOnly();

        public IReadOnlyCollection<ConsumerConfiguration> Consumers => this.consumers.AsReadOnly();

        public void AddConsumers(IEnumerable<ConsumerConfiguration> configurations) => this.consumers.AddRange(configurations);

        public void AddProducers(IEnumerable<ProducerConfiguration> configurations) => this.producers.AddRange(configurations);
    }
}
