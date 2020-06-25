namespace KafkaFlow.Configuration
{
    public interface IConsumerMiddlewareConfigurationBuilder
    {
        IDependencyConfigurator DependencyConfigurator { get; }

        /// <summary>
        /// Register a middleware
        /// </summary>
        /// <param name="factory">A factory to create the instance</param>
        /// <typeparam name="T">A class that implements the <see cref="IMessageMiddleware"/></typeparam>
        /// <returns></returns>
        IConsumerMiddlewareConfigurationBuilder Add<T>(Factory<T> factory)
            where T : class, IMessageMiddleware;

        /// <summary>
        /// Register a middleware
        /// </summary>
        /// <typeparam name="T">A class that implements the <see cref="IMessageMiddleware"/></typeparam>
        /// <returns></returns>
        IConsumerMiddlewareConfigurationBuilder Add<T>()
            where T : class, IMessageMiddleware;
    }
}
