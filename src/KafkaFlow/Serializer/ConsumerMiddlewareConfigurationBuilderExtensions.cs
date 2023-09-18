namespace KafkaFlow.Serializer
{
    using System;
    using KafkaFlow.Configuration;

    /// <summary>
    /// No needed
    /// </summary>
    public static class ConsumerMiddlewareConfigurationBuilderExtensions
    {
        /// <summary>
        /// Registers a middleware to deserialize messages
        /// </summary>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <typeparam name="TSerializer">A class that implements <see cref="ISerializer"/></typeparam>
        /// <typeparam name="TResolver">A class that implements <see cref="IMessageTypeResolver"/></typeparam>
        /// <returns></returns>
        public static IConsumerMiddlewareConfigurationBuilder AddSerializer<TSerializer, TResolver>(
            this IConsumerMiddlewareConfigurationBuilder middlewares)
            where TSerializer : class, ISerializer
            where TResolver : class, IMessageTypeResolver
        {
            middlewares.DependencyConfigurator.AddTransient<TResolver>();
            middlewares.DependencyConfigurator.AddTransient<TSerializer>();

            return middlewares.AddSerializer(
                resolver => resolver.Resolve<TSerializer>(),
                resolver => resolver.Resolve<TResolver>());
        }

        /// <summary>
        /// Register a middleware to deserialize messages
        /// </summary>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <typeparam name="TSerializer">A class that implements <see cref="ISerializer"/></typeparam>
        /// <typeparam name="TResolver">A class that implements <see cref="IMessageTypeResolver"/></typeparam>
        /// <param name="serializerFactory">A factory to create a <see cref="ISerializer"/></param>
        /// <param name="resolverFactory">A factory to create a <see cref="IMessageTypeResolver"/></param>
        /// <returns></returns>
        public static IConsumerMiddlewareConfigurationBuilder AddSerializer<TSerializer, TResolver>(
            this IConsumerMiddlewareConfigurationBuilder middlewares,
            Factory<TSerializer> serializerFactory,
            Factory<TResolver> resolverFactory)
            where TSerializer : class, ISerializer
            where TResolver : class, IMessageTypeResolver
        {
            return middlewares.Add(
                resolver => new SerializerConsumerMiddleware(
                    serializerFactory(resolver),
                    resolverFactory(resolver)));
        }

        /// <summary>
        /// Registers a middleware to deserialize messages
        /// </summary>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <typeparam name="TSerializer">A class that implements <see cref="ISerializer"/></typeparam>
        /// <returns></returns>
        public static IConsumerMiddlewareConfigurationBuilder AddSerializer<TSerializer>(
            this IConsumerMiddlewareConfigurationBuilder middlewares)
            where TSerializer : class, ISerializer
        {
            middlewares.DependencyConfigurator.AddTransient<TSerializer>();

            return middlewares.AddSerializer(
                resolver => resolver.Resolve<TSerializer>(),
                _ => new DefaultTypeResolver());
        }

        /// <summary>
        /// Register a middleware to deserialize messages
        /// </summary>
        /// <typeparam name="TSerializer">A class that implements <see cref="ISerializer"/></typeparam>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <param name="serializerFactory">A factory to create a <see cref="ISerializer"/></param>
        /// <returns></returns>
        public static IConsumerMiddlewareConfigurationBuilder AddSerializer<TSerializer>(
            this IConsumerMiddlewareConfigurationBuilder middlewares,
            Factory<TSerializer> serializerFactory)
            where TSerializer : class, ISerializer
        {
            return middlewares.AddSerializer(
                serializerFactory,
                _ => new DefaultTypeResolver());
        }

        /// <summary>
        /// Register a middleware to deserialize the message to a fixed type
        /// </summary>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <param name="serializerFactory">A factory to create a <see cref="ISerializer"/></param>
        /// <typeparam name="TMessage">The message type</typeparam>
        /// <typeparam name="TSerializer">A class that implements <see cref="ISerializer"/></typeparam>
        /// <returns></returns>
        public static IConsumerMiddlewareConfigurationBuilder AddSingleTypeSerializer<TMessage, TSerializer>(
            this IConsumerMiddlewareConfigurationBuilder middlewares,
            Factory<TSerializer> serializerFactory)
            where TSerializer : class, ISerializer
        {
            return middlewares.AddSerializer(
                serializerFactory,
                _ => new SingleMessageTypeResolver(typeof(TMessage)));
        }

        /// <summary>
        /// Register a middleware to deserialize the message to a fixed type
        /// </summary>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <typeparam name="TMessage">The message type</typeparam>
        /// <typeparam name="TSerializer">A class that implements <see cref="ISerializer"/></typeparam>
        /// <returns></returns>
        public static IConsumerMiddlewareConfigurationBuilder AddSingleTypeSerializer<TMessage, TSerializer>(
            this IConsumerMiddlewareConfigurationBuilder middlewares)
            where TSerializer : class, ISerializer
        {
            return middlewares.AddSingleTypeSerializer<TSerializer>(typeof(TMessage));
        }

        /// <summary>
        /// Register a middleware to deserialize the message to a fixed type
        /// </summary>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <param name="messageType">The message type</param>
        /// <typeparam name="TSerializer">A class that implements <see cref="ISerializer"/></typeparam>
        /// <returns></returns>
        public static IConsumerMiddlewareConfigurationBuilder AddSingleTypeSerializer<TSerializer>(
            this IConsumerMiddlewareConfigurationBuilder middlewares,
            Type messageType)
            where TSerializer : class, ISerializer
        {
            middlewares.DependencyConfigurator.AddTransient<TSerializer>();

            return middlewares.AddSerializer(
                resolver => resolver.Resolve<TSerializer>(),
                _ => new SingleMessageTypeResolver(messageType));
        }

        /// <summary>
        /// Register a middleware to deserialize the message to a fixed type
        /// </summary>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <param name="serializerFactory">A factory to create a <see cref="ISerializer"/></param>
        /// <param name="messageType">The message type</param>
        /// <typeparam name="TSerializer">A class that implements <see cref="ISerializer"/></typeparam>
        /// <returns></returns>
        public static IConsumerMiddlewareConfigurationBuilder AddSingleTypeSerializer<TSerializer>(
            this IConsumerMiddlewareConfigurationBuilder middlewares,
            Factory<TSerializer> serializerFactory,
            Type messageType)
            where TSerializer : class, ISerializer
        {
            return middlewares.AddSerializer(
                serializerFactory,
                _ => new SingleMessageTypeResolver(messageType));
        }
    }
}
