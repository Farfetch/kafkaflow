namespace KafkaFlow.Compressor
{
    using KafkaFlow.Configuration;

    /// <summary>
    /// Extension methods for <see cref="IConsumerMiddlewareConfigurationBuilder"/> and <see cref="IProducerMiddlewareConfigurationBuilder"/> implementations.
    /// </summary>
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// Registers a middleware to decompress the message
        /// </summary>
        public static IConsumerMiddlewareConfigurationBuilder AddCompressor<T>(
            this IConsumerMiddlewareConfigurationBuilder middlewares)
            where T : class, IMessageCompressor
        {
            return middlewares.AddCompressor(resolver => resolver.Resolve<T>());
        }

        /// <summary>
        /// Registers a middleware to decompress the message
        /// </summary>
        /// <param name="middlewares"></param>
        /// <param name="factory">A factory to create the <see cref="IMessageCompressor"/> instance</param>
        /// <returns></returns>
        public static IConsumerMiddlewareConfigurationBuilder AddCompressor<T>(
            this IConsumerMiddlewareConfigurationBuilder middlewares,
            Factory<T> factory)
            where T : class, IMessageCompressor
        {
            middlewares.DependencyConfigurator.AddSingleton<IMessageCompressor, T>();
            middlewares.DependencyConfigurator.AddSingleton<T>();

            return middlewares.Add(
                provider => new CompressorConsumerMiddleware(factory(provider)));
        }

        /// <summary>
        /// Registers a middleware to compress the message
        /// </summary>
        /// <typeparam name="T">A class that implements <see cref="IMessageCompressor"/></typeparam>
        public static IProducerMiddlewareConfigurationBuilder AddCompressor<T>(
            this IProducerMiddlewareConfigurationBuilder middlewares)
            where T : class, IMessageCompressor
        {
            return middlewares.AddCompressor(resolver => resolver.Resolve<T>());
        }

        /// <summary>
        /// Registers a middleware to compress the message
        /// </summary>
        /// <param name="middlewares"></param>
        /// <param name="factory">A factory to create the <see cref="IMessageCompressor"/> instance</param>
        public static IProducerMiddlewareConfigurationBuilder AddCompressor<T>(
            this IProducerMiddlewareConfigurationBuilder middlewares,
            Factory<T> factory)
            where T : class, IMessageCompressor
        {
            middlewares.DependencyConfigurator.AddSingleton<IMessageCompressor, T>();
            middlewares.DependencyConfigurator.AddSingleton<T>();

            return middlewares.Add(
                provider => new CompressorProducerMiddleware(factory(provider)));
        }
    }
}
