namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// </summary>
    public interface IClusterConfigurationBuilder
    {
        /// <summary>
        /// Set the Kafka Brokers to be used
        /// </summary>
        /// <param name="brokers"></param>
        /// <returns></returns>
        IClusterConfigurationBuilder WithBrokers(IEnumerable<string> brokers);

        /// <summary>
        /// Adds a producer to the cluster
        /// </summary>
        /// <param name="producer">A handler to configure the producer</param>
        /// <typeparam name="TProducer">The class responsible for the production</typeparam>
        /// <returns></returns>
        IClusterConfigurationBuilder AddProducer<TProducer>(Action<IProducerConfigurationBuilder> producer);

        /// <summary>
        /// Adds a consumer to the cluster
        /// </summary>
        /// <param name="consumer">A handler to configure the consumer</param>
        /// <returns></returns>
        IClusterConfigurationBuilder AddConsumer(Action<IConsumerConfigurationBuilder> consumer);
    }
}
