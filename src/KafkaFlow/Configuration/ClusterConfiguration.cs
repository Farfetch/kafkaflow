namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// Represents the cluster configuration values
    /// </summary>
    public class ClusterConfiguration
    {
        private readonly Func<SecurityInformation> securityInformationHandler;
        private readonly List<IProducerConfiguration> producers = new();
        private readonly List<IConsumerConfiguration> consumers = new();
        private readonly ReadOnlyCollection<TopicConfiguration> topicsToCreateIfNotExist;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClusterConfiguration"/> class.
        /// </summary>
        /// <param name="kafka">The kafka configuration</param>
        /// <param name="name">The cluster name</param>
        /// <param name="brokers">The list of brokers</param>
        /// <param name="securityInformationHandler">The security information handler</param>
        /// <param name="onStartedHandler">The handler to be executed when the cluster started</param>
        /// <param name="onStoppingHandler">The handler to be executed when the cluster is stopping</param>
        /// <param name="topicsToCreateIfNotExist">Topics to create on startup if not exists</param>
        public ClusterConfiguration(
            KafkaConfiguration kafka,
            string name,
            IEnumerable<string> brokers,
            Func<SecurityInformation> securityInformationHandler,
            Action<IDependencyResolver> onStartedHandler,
            Action<IDependencyResolver> onStoppingHandler,
            IEnumerable<TopicConfiguration> topicsToCreateIfNotExist = null)
        {
            this.securityInformationHandler = securityInformationHandler;
            this.Name = name ?? Guid.NewGuid().ToString();
            this.Kafka = kafka;
            this.Brokers = brokers.ToList();
            this.OnStoppingHandler = onStoppingHandler;
            this.OnStartedHandler = onStartedHandler;
            this.topicsToCreateIfNotExist = topicsToCreateIfNotExist?.ToList().AsReadOnly() ??
                                            new List<TopicConfiguration>().AsReadOnly();
        }

        /// <summary>
        /// Gets the kafka configuration
        /// </summary>
        public KafkaConfiguration Kafka { get; }

        /// <summary>
        /// Gets the list of brokers
        /// </summary>
        public IReadOnlyCollection<string> Brokers { get; }

        /// <summary>
        /// Gets the cluster name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the list of producers
        /// </summary>
        public IReadOnlyCollection<IProducerConfiguration> Producers => this.producers.AsReadOnly();

        /// <summary>
        /// Gets the list of consumers
        /// </summary>
        public IReadOnlyCollection<IConsumerConfiguration> Consumers => this.consumers.AsReadOnly();

        /// <summary>
        /// Gets the list of topics to create if they do not exist
        /// </summary>
        public IReadOnlyCollection<TopicConfiguration> TopicsToCreateIfNotExist =>
            this.topicsToCreateIfNotExist;

        /// <summary>
        /// Gets the handler to be executed when the cluster started
        /// </summary>
        public Action<IDependencyResolver> OnStartedHandler { get; }

        /// <summary>
        /// Gets the handler to be executed when the cluster is stopping
        /// </summary>
        public Action<IDependencyResolver> OnStoppingHandler { get; }

        /// <summary>
        /// Adds a list of consumer configurations
        /// </summary>
        /// <param name="configurations">A list of consumer configurations</param>
        public void AddConsumers(IEnumerable<IConsumerConfiguration> configurations) =>
            this.consumers.AddRange(configurations);

        /// <summary>
        /// Adds a list of producer configurations
        /// </summary>
        /// <param name="configurations">A list of producer configurations</param>
        public void AddProducers(IEnumerable<IProducerConfiguration> configurations) =>
            this.producers.AddRange(configurations);

        /// <summary>
        /// Gets the kafka security information
        /// </summary>
        /// <returns></returns>
        public SecurityInformation GetSecurityInformation() => this.securityInformationHandler?.Invoke();
    }
}
