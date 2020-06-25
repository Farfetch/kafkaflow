namespace KafkaFlow.Configuration
{
    using System;

    public interface IProducerConfigurationBuilder
    {
        IDependencyConfigurator DependencyConfigurator { get; }

        /// <summary>
        /// Adds middlewares to the producer. The middlewares will be executed in the registration order
        /// </summary>
        /// <param name="middlewares">A handler to register middlewares</param>
        /// <returns></returns>
        IProducerConfigurationBuilder AddMiddlewares(Action<IProducerMiddlewareConfigurationBuilder> middlewares);

        /// <summary>
        /// Set the default topic to be used when producing messages
        /// </summary>
        /// <param name="topic">Topic name</param>
        /// <returns></returns>
        IProducerConfigurationBuilder DefaultTopic(string topic);

        /// <summary>
        /// Set the <see cref="Acks"/> to be used when producing messages
        /// </summary>
        /// <param name="acks"></param>
        /// <returns></returns>
        IProducerConfigurationBuilder WithAcks(Acks acks);
    }
}
