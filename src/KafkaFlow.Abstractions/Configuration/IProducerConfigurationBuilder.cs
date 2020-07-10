namespace KafkaFlow.Configuration
{
    using System;

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
        /// <param name="acks"></param>
        /// <returns></returns>
        IProducerConfigurationBuilder WithAcks(Acks acks);

        /// <summary>
        /// Sets a unique name for the producer
        /// </summary>
        /// <param name="name">A unique name</param>
        /// <returns></returns>
        IProducerConfigurationBuilder WithName(string name);
    }
}
