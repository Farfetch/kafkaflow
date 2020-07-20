namespace KafkaFlow.Configuration
{
    using System;
    using Confluent.Kafka;
    using Acks = KafkaFlow.Acks;

    internal class ProducerConfiguration
    {
        public ProducerConfiguration(
            ClusterConfiguration cluster,
            string name,
            string defaultTopic,
            Acks? acks,
            MiddlewareConfiguration middlewareConfiguration,
            ProducerConfig baseProducerConfig)
        {
            this.Cluster = cluster ?? throw new ArgumentNullException(nameof(cluster));
            this.Name = name;
            this.DefaultTopic = defaultTopic;
            this.Acks = acks;
            this.MiddlewareConfiguration = middlewareConfiguration;
            this.BaseProducerConfig = baseProducerConfig;
        }

        public ClusterConfiguration Cluster { get; }

        public string Name { get; }

        public string DefaultTopic { get; }

        public ProducerConfig BaseProducerConfig { get; }

        public Acks? Acks { get; }

        public MiddlewareConfiguration MiddlewareConfiguration { get; }

        public ProducerConfig GetKafkaConfig()
        {
            this.BaseProducerConfig.BootstrapServers = string.Join(",", this.Cluster.Brokers);
            this.BaseProducerConfig.Acks = ParseAcks(this.Acks);

            return this.BaseProducerConfig;
        }

        private static Confluent.Kafka.Acks? ParseAcks(Acks? acks)
        {
            switch (acks)
            {
                case KafkaFlow.Acks.Leader:
                    return Confluent.Kafka.Acks.Leader;

                case KafkaFlow.Acks.All:
                    return Confluent.Kafka.Acks.All;

                case KafkaFlow.Acks.None:
                    return Confluent.Kafka.Acks.None;

                default:
                    return null;
            }
        }
    }
}
