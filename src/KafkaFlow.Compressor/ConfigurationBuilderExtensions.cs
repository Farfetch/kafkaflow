namespace KafkaFlow.Compressor
{
    using System;
    using KafkaFlow.Configuration;

    /// <summary>
    /// Extension methods for <see cref="IConsumerMiddlewareConfigurationBuilder"/> and <see cref="IProducerMiddlewareConfigurationBuilder"/>.
    /// </summary>
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// Registers a middleware to decompress the message
        /// </summary>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <typeparam name="T">The compressor type</typeparam>
        /// <returns></returns>
        [Obsolete("Compressors should only be used in backward compatibility scenarios, in the vast majority of cases native compression (producer.WithCompression()) should be used instead")]
        public static IConsumerMiddlewareConfigurationBuilder AddCompressor<T>(this IConsumerMiddlewareConfigurationBuilder middlewares)
            where T : class, IMessageCompressor
        {
            middlewares.DependencyConfigurator.AddTransient<T>();
            return middlewares.AddCompressor(resolver => resolver.Resolve<T>());
        }

        /// <summary>
        /// Registers a middleware to decompress the message
        /// </summary>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <typeparam name="T">The compressor type that implements <see cref="IMessageCompressor"/></typeparam>
        /// <param name="factory">A factory to create the <see cref="IMessageCompressor"/> instance</param>
        /// <returns></returns>
        [Obsolete("Compressors should only be used in backward compatibility scenarios, in the vast majority of cases native compression (producer.WithCompression()) should be used instead")]
        public static IConsumerMiddlewareConfigurationBuilder AddCompressor<T>(
            this IConsumerMiddlewareConfigurationBuilder middlewares,
            Factory<T> factory)
            where T : class, IMessageCompressor
        {
            return middlewares.Add(resolver => new CompressorConsumerMiddleware(factory(resolver)));
        }

        /// <summary>
        /// Registers a middleware to compress the message
        /// It is highly recommended to use the producer native compression ('WithCompression()' method) instead of using the compressor middleware
        /// </summary>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <typeparam name="T">The compressor type that implements <see cref="IMessageCompressor"/></typeparam>
        /// <returns></returns>
        [Obsolete("Compressors should only be used in backward compatibility scenarios, in the vast majority of cases native compression (producer.WithCompression()) should be used instead")]
        public static IProducerMiddlewareConfigurationBuilder AddCompressor<T>(this IProducerMiddlewareConfigurationBuilder middlewares)
            where T : class, IMessageCompressor
        {
            middlewares.DependencyConfigurator.AddTransient<T>();
            return middlewares.AddCompressor(resolver => resolver.Resolve<T>());
        }

        /// <summary>
        /// Registers a middleware to compress the message
        /// It is highly recommended to use the producer native compression ('WithCompression()' method) instead of using the compressor middleware
        /// </summary>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <typeparam name="T">The compressor type that implements <see cref="IMessageCompressor"/></typeparam>
        /// <param name="factory">A factory to create the <see cref="IMessageCompressor"/> instance</param>
        /// <returns></returns>
        [Obsolete("Compressors should only be used in backward compatibility scenarios, in the vast majority of cases native compression (producer.WithCompression()) should be used instead")]
        public static IProducerMiddlewareConfigurationBuilder AddCompressor<T>(
            this IProducerMiddlewareConfigurationBuilder middlewares,
            Factory<T> factory)
            where T : class, IMessageCompressor
        {
            return middlewares.Add(resolver => new CompressorProducerMiddleware(factory(resolver)));
        }
    }
}
