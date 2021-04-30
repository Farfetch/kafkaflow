namespace KafkaFlow
{
    using KafkaFlow.Configuration;

    /// <summary>
    /// Extension methods for <see cref="IConsumerMiddlewareConfigurationBuilder"/> and <see cref="IProducerMiddlewareConfigurationBuilder"/>
    /// </summary>
    public static class ConfigurationBuilderExtensions
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

            return middlewares.Add(
                resolver => new SerializerConsumerMiddleware(
                    resolver.Resolve<TSerializer>(),
                    resolver.Resolve<TResolver>()));
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
                resolver => new DefaultMessageTypeResolver());
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
                resolver => new DefaultMessageTypeResolver());
        }

        /// <summary>
        /// Registers a middleware to serialize messages
        /// </summary>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <typeparam name="TSerializer">A class that implements <see cref="ISerializer"/></typeparam>
        /// <typeparam name="TResolver">A class that implements <see cref="IMessageTypeResolver"/></typeparam>
        /// <returns></returns>
        public static IProducerMiddlewareConfigurationBuilder AddSerializer<TSerializer, TResolver>(
            this IProducerMiddlewareConfigurationBuilder middlewares)
            where TSerializer : class, ISerializer
            where TResolver : class, IMessageTypeResolver
        {
            middlewares.DependencyConfigurator.AddTransient<TResolver>();
            middlewares.DependencyConfigurator.AddTransient<TSerializer>();

            return middlewares.Add(
                resolver => new SerializerProducerMiddleware(
                    resolver.Resolve<TSerializer>(),
                    resolver.Resolve<TResolver>()));
        }

        /// <summary>
        /// Registers a middleware to serialize messages
        /// </summary>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <typeparam name="TSerializer">A class that implements <see cref="ISerializer"/></typeparam>
        /// <typeparam name="TResolver">A class that implements <see cref="IMessageTypeResolver"/></typeparam>
        /// <param name="serializerFactory">A factory to create a <see cref="ISerializer"/></param>
        /// <param name="resolverFactory">A factory to create a <see cref="IMessageTypeResolver"/></param>
        /// <returns></returns>
        public static IProducerMiddlewareConfigurationBuilder AddSerializer<TSerializer, TResolver>(
            this IProducerMiddlewareConfigurationBuilder middlewares,
            Factory<TSerializer> serializerFactory,
            Factory<TResolver> resolverFactory)
            where TSerializer : class, ISerializer
            where TResolver : class, IMessageTypeResolver
        {
            return middlewares.Add(
                resolver => new SerializerProducerMiddleware(
                    serializerFactory(resolver),
                    resolverFactory(resolver)));
        }

        /// <summary>
        /// Registers a middleware to serialize messages
        /// </summary>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <typeparam name="TSerializer">A class that implements <see cref="ISerializer"/></typeparam>
        /// <returns></returns>x
        public static IProducerMiddlewareConfigurationBuilder AddSerializer<TSerializer>(
            this IProducerMiddlewareConfigurationBuilder middlewares)
            where TSerializer : class, ISerializer
        {
            middlewares.DependencyConfigurator.AddTransient<TSerializer>();

            return middlewares.Add(
                resolver => new SerializerProducerMiddleware(
                    resolver.Resolve<TSerializer>(),
                    new DefaultMessageTypeResolver()));
        }

        /// <summary>
        /// Registers a middleware to serialize messages
        /// </summary>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <typeparam name="TSerializer">A class that implements <see cref="ISerializer"/></typeparam>
        /// <param name="serializerFactory">A factory to create a <see cref="ISerializer"/></param>
        /// <returns></returns>
        public static IProducerMiddlewareConfigurationBuilder AddSerializer<TSerializer>(
            this IProducerMiddlewareConfigurationBuilder middlewares,
            Factory<TSerializer> serializerFactory)
            where TSerializer : class, ISerializer
        {
            return middlewares.AddSerializer(
                serializerFactory,
                resolver => new DefaultMessageTypeResolver());
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
                resolver => new SingleMessageTypeResolver<TMessage>());
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
            return middlewares.AddSerializer<TSerializer, SingleMessageTypeResolver<TMessage>>();
        }

        /// <summary>
        /// Register a middleware to serialize the message to a fixed type
        /// </summary>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <param name="serializerFactory">A factory to create a <see cref="ISerializer"/></param>
        /// <typeparam name="TMessage">The message type</typeparam>
        /// <typeparam name="TSerializer">A class that implements <see cref="ISerializer"/></typeparam>
        /// <returns></returns>
        public static IProducerMiddlewareConfigurationBuilder AddSingleTypeSerializer<TMessage, TSerializer>(
            this IProducerMiddlewareConfigurationBuilder middlewares,
            Factory<TSerializer> serializerFactory)
            where TSerializer : class, ISerializer
        {
            return middlewares.AddSerializer(
                serializerFactory,
                resolver => new SingleMessageTypeResolver<TMessage>());
        }

        /// <summary>
        /// Register a middleware to serialize the message to a fixed type
        /// </summary>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <typeparam name="TMessage">The message type</typeparam>
        /// <typeparam name="TSerializer">A class that implements <see cref="ISerializer"/></typeparam>
        /// <returns></returns>
        public static IProducerMiddlewareConfigurationBuilder AddSingleTypeSerializer<TMessage, TSerializer>(
            this IProducerMiddlewareConfigurationBuilder middlewares)
            where TSerializer : class, ISerializer
        {
            return middlewares.AddSerializer<TSerializer, SingleMessageTypeResolver<TMessage>>();
        }
    }
}
