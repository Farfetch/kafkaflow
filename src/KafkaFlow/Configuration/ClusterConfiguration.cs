namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class ClusterConfiguration
    {
        private readonly Func<SecurityInformation> securityInformationHandler;
        private readonly Action<IDependencyResolver> onStopHandler = _ => { };
        private readonly List<IProducerConfiguration> producers = new();
        private readonly List<IConsumerConfiguration> consumers = new();

        public ClusterConfiguration(
            KafkaConfiguration kafka,
            IEnumerable<string> brokers,
            Func<SecurityInformation> securityInformationHandler,
            Action<IDependencyResolver> onStopHandler)
        {
            this.securityInformationHandler = securityInformationHandler;
            this.Kafka = kafka;
            this.Brokers = brokers.ToList();
            this.onStopHandler = onStopHandler;
        }

        public KafkaConfiguration Kafka { get; }

        public IReadOnlyCollection<string> Brokers { get; }

        public IReadOnlyCollection<IProducerConfiguration> Producers => this.producers.AsReadOnly();

        public IReadOnlyCollection<IConsumerConfiguration> Consumers => this.consumers.AsReadOnly();

        public Action<IDependencyResolver> OnStopHandler => this.onStopHandler;

        public void AddConsumers(IEnumerable<IConsumerConfiguration> configurations) => this.consumers.AddRange(configurations);

        public void AddProducers(IEnumerable<IProducerConfiguration> configurations) => this.producers.AddRange(configurations);

        public SecurityInformation GetSecurityInformation() => this.securityInformationHandler?.Invoke();
    }
}
