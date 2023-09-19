namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A interface to build the cluster configuration
    /// </summary>
    public interface IClusterConfigurationBuilder
    {
        /// <summary>
        /// Gets the dependency injection configurator
        /// </summary>
        IDependencyConfigurator DependencyConfigurator { get; }

        /// <summary>
        /// Set the Kafka Brokers to be used
        /// </summary>
        /// <param name="brokers">The brokers address to be used</param>
        /// <returns></returns>
        IClusterConfigurationBuilder WithBrokers(IEnumerable<string> brokers);

        /// <summary>
        /// Sets a unique name for the cluster
        /// </summary>
        /// <param name="name">A unique name</param>
        /// <returns></returns>
        IClusterConfigurationBuilder WithName(string name);

        /// <summary>
        /// Configures cluster security
        /// </summary>
        /// <param name="handler">A handler to sets the values</param>
        /// <returns></returns>
        IClusterConfigurationBuilder WithSecurityInformation(Action<SecurityInformation> handler);

        /// <summary>
        /// Adds a producer to the cluster
        /// </summary>
        /// <param name="producer">A handler to configure the producer</param>
        /// <typeparam name="TProducer">A type used to get the internal producer instance</typeparam>
        /// <returns></returns>
        IClusterConfigurationBuilder AddProducer<TProducer>(Action<IProducerConfigurationBuilder> producer);

        /// <summary>
        /// Adds a producer to the cluster
        /// </summary>
        /// <param name="name">The producer name used to get its instance</param>
        /// <param name="producer">A handler to configure the producer</param>
        /// <returns></returns>
        IClusterConfigurationBuilder AddProducer(string name, Action<IProducerConfigurationBuilder> producer);

        /// <summary>
        /// Adds a consumer to the cluster
        /// </summary>
        /// <param name="consumer">A handler to configure the consumer</param>
        /// <returns></returns>
        IClusterConfigurationBuilder AddConsumer(Action<IConsumerConfigurationBuilder> consumer);

        /// <summary>
        /// Adds a handler to be executed when KafkaFlow cluster is stopping
        /// </summary>
        /// <param name="handler">A handler to KafkaFlow cluster stopping event</param>
        /// <returns></returns>
        IClusterConfigurationBuilder OnStopping(Action<IDependencyResolver> handler);

        /// <summary>
        /// Adds a handler to be executed after KafkaFlow cluster started
        /// </summary>
        /// <param name="handler">A handler to KafkaFlow cluster started event</param>
        /// <returns></returns>
        IClusterConfigurationBuilder OnStarted(Action<IDependencyResolver> handler);

        /// <summary>
        /// Adds a Topic to the Cluster
        /// </summary>
        /// <param name="topicName">The topic name</param>
        /// <param name="numberOfPartitions">The number of Topic partitions</param>
        /// <param name="replicationFactor">The Topic replication factor</param>
        /// <returns></returns>
        IClusterConfigurationBuilder CreateTopicIfNotExists(
            string topicName,
            int numberOfPartitions,
            short replicationFactor);

        // Gets the cluster configuration
        IClusterConfigurationBuilder AddInstrumentation<TConsumerInstrumentationMiddleware, TProducerInstrumentationMiddleware>()
            where TConsumerInstrumentationMiddleware : class, IMessageMiddleware
            where TProducerInstrumentationMiddleware : class, IMessageMiddleware;
    }
}
