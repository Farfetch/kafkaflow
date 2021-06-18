namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class ClusterConfiguration
    {
        private readonly Func<SecurityInformation> securityInformationHandler;
        private readonly Action<IDependencyResolver> onStartedHandler;
        private readonly Action<IDependencyResolver> onStoppingHandler;
        private readonly List<IProducerConfiguration> producers = new();
        private readonly List<IConsumerConfiguration> consumers = new();

        public ClusterConfiguration(
            KafkaConfiguration kafka,
            string name,
            IEnumerable<string> brokers,
            Func<SecurityInformation> securityInformationHandler,
            Action<IDependencyResolver> onStartedHandler,
            Action<IDependencyResolver> onStoppingHandler)
        {
            this.securityInformationHandler = securityInformationHandler;
            this.Name = name ?? Guid.NewGuid().ToString();
            this.Kafka = kafka;
            this.Brokers = brokers.ToList();
            this.onStoppingHandler = onStoppingHandler;
            this.onStartedHandler = onStartedHandler;
        }

        public KafkaConfiguration Kafka { get; }

        public IReadOnlyCollection<string> Brokers { get; }

        public string Name { get; }

        public IReadOnlyCollection<IProducerConfiguration> Producers => this.producers.AsReadOnly();

        public IReadOnlyCollection<IConsumerConfiguration> Consumers => this.consumers.AsReadOnly();

        public Action<IDependencyResolver> OnStartedHandler => this.onStartedHandler;

        public Action<IDependencyResolver> OnStoppingHandler => this.onStoppingHandler;

        public void AddConsumers(IEnumerable<IConsumerConfiguration> configurations) =>
            this.consumers.AddRange(configurations);

        public void AddProducers(IEnumerable<IProducerConfiguration> configurations) =>
            this.producers.AddRange(configurations);

        public SecurityInformation GetSecurityInformation() => this.securityInformationHandler?.Invoke();
    }
}
