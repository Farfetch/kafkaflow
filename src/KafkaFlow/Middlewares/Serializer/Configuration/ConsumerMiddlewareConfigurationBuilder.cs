namespace KafkaFlow
{
    using System;
    using KafkaFlow.Configuration;
    using KafkaFlow.Middlewares.Serializer;
    using KafkaFlow.Middlewares.Serializer.Resolvers;

    /// <summary>
    /// No needed
    /// </summary>
    public static class ConsumerMiddlewareConfigurationBuilder
    {
        /// <summary>
        /// Registers a middleware to deserialize messages
        /// </summary>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <typeparam name="TDeserializer">A class that implements <see cref="IDeserializer"/></typeparam>
        /// <typeparam name="TResolver">A class that implements <see cref="IMessageTypeResolver"/></typeparam>
        /// <returns></returns>
        public static IConsumerMiddlewareConfigurationBuilder AddDeserializer<TDeserializer, TResolver>(
            this IConsumerMiddlewareConfigurationBuilder middlewares)
            where TDeserializer : class, IDeserializer
            where TResolver : class, IMessageTypeResolver
        {
            middlewares.DependencyConfigurator.AddTransient<TResolver>();
            middlewares.DependencyConfigurator.AddTransient<TDeserializer>();

            return middlewares.AddDeserializer(
                resolver => resolver.Resolve<TDeserializer>(),
                resolver => resolver.Resolve<TResolver>());
        }

        /// <summary>
        /// Register a middleware to deserialize messages
        /// </summary>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <typeparam name="TDeserializer">A class that implements <see cref="IDeserializer"/></typeparam>
        /// <typeparam name="TResolver">A class that implements <see cref="IMessageTypeResolver"/></typeparam>
        /// <param name="serializerFactory">A factory to create a <see cref="IDeserializer"/></param>
        /// <param name="resolverFactory">A factory to create a <see cref="IMessageTypeResolver"/></param>
        /// <returns></returns>
        public static IConsumerMiddlewareConfigurationBuilder AddDeserializer<TDeserializer, TResolver>(
            this IConsumerMiddlewareConfigurationBuilder middlewares,
            Factory<TDeserializer> serializerFactory,
            Factory<TResolver> resolverFactory)
            where TDeserializer : class, IDeserializer
            where TResolver : class, IMessageTypeResolver
        {
            return middlewares.Add(
                resolver => new DeserializerConsumerMiddleware(
                    serializerFactory(resolver),
                    resolverFactory(resolver)));
        }

        /// <summary>
        /// Registers a middleware to deserialize messages
        /// </summary>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <typeparam name="TDeserializer">A class that implements <see cref="IDeserializer"/></typeparam>
        /// <returns></returns>
        public static IConsumerMiddlewareConfigurationBuilder AddDeserializer<TDeserializer>(
            this IConsumerMiddlewareConfigurationBuilder middlewares)
            where TDeserializer : class, IDeserializer
        {
            middlewares.DependencyConfigurator.AddTransient<TDeserializer>();

            return middlewares.AddDeserializer(
                resolver => resolver.Resolve<TDeserializer>(),
                _ => new DefaultTypeResolver());
        }

        /// <summary>
        /// Register a middleware to deserialize messages
        /// </summary>
        /// <typeparam name="TDeserializer">A class that implements <see cref="IDeserializer"/></typeparam>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <param name="serializerFactory">A factory to create a <see cref="IDeserializer"/></param>
        /// <returns></returns>
        public static IConsumerMiddlewareConfigurationBuilder AddDeserializer<TDeserializer>(
            this IConsumerMiddlewareConfigurationBuilder middlewares,
            Factory<TDeserializer> serializerFactory)
            where TDeserializer : class, IDeserializer
        {
            return middlewares.AddDeserializer(
                serializerFactory,
                _ => new DefaultTypeResolver());
        }

        /// <summary>
        /// Register a middleware to deserialize the message to a fixed type
        /// </summary>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <param name="serializerFactory">A factory to create a <see cref="IDeserializer"/></param>
        /// <typeparam name="TMessage">The message type</typeparam>
        /// <typeparam name="TDeserializer">A class that implements <see cref="IDeserializer"/></typeparam>
        /// <returns></returns>
        public static IConsumerMiddlewareConfigurationBuilder AddSingleTypeDeserializer<TMessage, TDeserializer>(
            this IConsumerMiddlewareConfigurationBuilder middlewares,
            Factory<TDeserializer> serializerFactory)
            where TDeserializer : class, IDeserializer
        {
            return middlewares.AddDeserializer(
                serializerFactory,
                _ => new SingleMessageTypeResolver(typeof(TMessage)));
        }

        /// <summary>
        /// Register a middleware to deserialize the message to a fixed type
        /// </summary>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <typeparam name="TMessage">The message type</typeparam>
        /// <typeparam name="TDeserializer">A class that implements <see cref="IDeserializer"/></typeparam>
        /// <returns></returns>
        public static IConsumerMiddlewareConfigurationBuilder AddSingleTypeDeserializer<TMessage, TDeserializer>(
            this IConsumerMiddlewareConfigurationBuilder middlewares)
            where TDeserializer : class, IDeserializer
        {
            return middlewares.AddSingleTypeDeserializer<TDeserializer>(typeof(TMessage));
        }

        /// <summary>
        /// Register a middleware to deserialize the message to a fixed type
        /// </summary>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <param name="messageType">The message type</param>
        /// <typeparam name="TDeserializer">A class that implements <see cref="IDeserializer"/></typeparam>
        /// <returns></returns>
        public static IConsumerMiddlewareConfigurationBuilder AddSingleTypeDeserializer<TDeserializer>(
            this IConsumerMiddlewareConfigurationBuilder middlewares,
            Type messageType)
            where TDeserializer : class, IDeserializer
        {
            middlewares.DependencyConfigurator.AddTransient<TDeserializer>();

            return middlewares.AddDeserializer(
                resolver => resolver.Resolve<TDeserializer>(),
                _ => new SingleMessageTypeResolver(messageType));
        }

        /// <summary>
        /// Register a middleware to deserialize the message to a fixed type
        /// </summary>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <param name="serializerFactory">A factory to create a <see cref="IDeserializer"/></param>
        /// <param name="messageType">The message type</param>
        /// <typeparam name="TDeserializer">A class that implements <see cref="IDeserializer"/></typeparam>
        /// <returns></returns>
        public static IConsumerMiddlewareConfigurationBuilder AddSingleTypeDeserializer<TDeserializer>(
            this IConsumerMiddlewareConfigurationBuilder middlewares,
            Factory<TDeserializer> serializerFactory,
            Type messageType)
            where TDeserializer : class, IDeserializer
        {
            return middlewares.AddDeserializer(
                serializerFactory,
                _ => new SingleMessageTypeResolver(messageType));
        }
    }
}
