using System;

namespace KafkaFlow.Configuration
{
    /// <summary>
    /// Used to build the producer configuration
    /// </summary>
    public interface IProducerConfigurationBuilder
    {
        /// <summary>
        /// Gets the dependency injection configurator
        /// </summary>
        IDependencyConfigurator DependencyConfigurator { get; }

        /// <summary>
        /// Adds middlewares to the producer. The middlewares will be executed in the registration order
        /// </summary>
        /// <param name="middlewares">A handler to register middlewares</param>
        /// <returns></returns>
        IProducerConfigurationBuilder AddMiddlewares(Action<IProducerMiddlewareConfigurationBuilder> middlewares);

        /// <summary>
        /// Sets the default topic to be used when producing messages
        /// </summary>
        /// <param name="topic">Topic name</param>
        /// <returns></returns>
        IProducerConfigurationBuilder DefaultTopic(string topic);

        /// <summary>
        /// Sets the <see cref="Acks"/> to be used when producing messages
        /// </summary>
        /// <param name="acks">The <see cref="Acks"/> enum value</param>
        /// <returns></returns>
        IProducerConfigurationBuilder WithAcks(Acks acks);

        /// <summary>
        /// Delay in milliseconds to wait for messages in the producer queue to accumulate before constructing message batches to transmit to brokers.
        /// A higher value allows larger and more effective (less overhead, improved compression) batches of messages to accumulate at the expense of increased message delivery latency.
        /// default: 0.5 (500 microseconds)
        /// importance: high
        /// </summary>
        /// <param name="lingerMs">The time in milliseconds to wait to build the message batch</param>
        /// <returns></returns>
        IProducerConfigurationBuilder WithLingerMs(double lingerMs);

        /// <summary>
        /// Adds a handler for the Kafka producer statistics
        /// </summary>
        /// <param name="statisticsHandler">A handler for the statistics</param>
        /// <returns></returns>
        IProducerConfigurationBuilder WithStatisticsHandler(Action<string> statisticsHandler);

        /// <summary>
        /// Sets the interval the statistics are emitted
        /// </summary>
        /// <param name="statisticsIntervalMs">The interval in miliseconds</param>
        /// <returns></returns>
        IProducerConfigurationBuilder WithStatisticsIntervalMs(int statisticsIntervalMs);
    }
}
