namespace KafkaFlow.Compressor
{
    using KafkaFlow.Configuration;

    /// <summary>
    /// Extension methods for <see cref="IConsumerMiddlewareConfigurationBuilder"/> and <see cref="IProducerMiddlewareConfigurationBuilder"/>.
    /// </summary>
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// Registers a middleware to decompress the message
        /// </summary>
        public static IConsumerMiddlewareConfigurationBuilder AddCompressor<T>(this IConsumerMiddlewareConfigurationBuilder middlewares)
            where T : class, IMessageCompressor
        {
            middlewares.DependencyConfigurator.AddTransient<T>();
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
            return middlewares.Add(resolver => new CompressorConsumerMiddleware(factory(resolver)));
        }

        /// <summary>
        /// Registers a middleware to compress the message
        /// </summary>
        /// <typeparam name="T">A class that implements <see cref="IMessageCompressor"/></typeparam>
        public static IProducerMiddlewareConfigurationBuilder AddCompressor<T>(this IProducerMiddlewareConfigurationBuilder middlewares)
            where T : class, IMessageCompressor
        {
            middlewares.DependencyConfigurator.AddTransient<T>();
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
            return middlewares.Add(resolver => new CompressorProducerMiddleware(factory(resolver)));
        }
    }
}
