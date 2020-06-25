namespace KafkaFlow.Serializer
{
    using KafkaFlow.Configuration;

    /// <summary>
    /// </summary>
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// Register a middleware to deserialize messages
        /// </summary>
        /// <typeparam name="TSerializer">A class that implements <see cref="IMessageSerializer"/></typeparam>
        /// <typeparam name="TResolver">A class that implements <see cref="IMessageTypeResolver"/></typeparam>
        public static IConsumerMiddlewareConfigurationBuilder AddSerializer<TSerializer, TResolver>(this IConsumerMiddlewareConfigurationBuilder consumer)
            where TSerializer : class, IMessageSerializer
            where TResolver : class, IMessageTypeResolver
        {
            return consumer.AddSerializer(
                resolver => resolver.Resolve<TSerializer>(),
                resolver => resolver.Resolve<TResolver>());
        }

        /// <summary>
        /// Register a middleware to deserialize messages
        /// </summary>
        /// <typeparam name="TSerializer">A class that implements <see cref="IMessageSerializer"/></typeparam>
        /// <typeparam name="TResolver">A class that implements <see cref="IMessageTypeResolver"/></typeparam>
        /// <param name="middlewares"></param>
        /// <param name="serializerFactory">A factory to create a <see cref="IMessageSerializer"/></param>
        /// <param name="resolverFactory">A factory to create a <see cref="IMessageTypeResolver"/></param>
        public static IConsumerMiddlewareConfigurationBuilder AddSerializer<TSerializer, TResolver>(
            this IConsumerMiddlewareConfigurationBuilder middlewares,
            Factory<TSerializer> serializerFactory,
            Factory<TResolver> resolverFactory)
            where TSerializer : class, IMessageSerializer
            where TResolver : class, IMessageTypeResolver
        {
            middlewares.DependencyConfigurator.AddSingleton<IMessageSerializer, TSerializer>();
            middlewares.DependencyConfigurator.AddSingleton<IMessageTypeResolver, TResolver>();
            middlewares.DependencyConfigurator.AddSingleton<TSerializer>();
            middlewares.DependencyConfigurator.AddSingleton<TResolver>();

            return middlewares.Add(
                provider => new SerializerConsumerMiddleware(
                    serializerFactory(provider),
                    resolverFactory(provider)));
        }

        /// <summary>
        /// Register a middleware to serialize messages
        /// </summary>
        /// <typeparam name="TSerializer">A class that implements <see cref="IMessageSerializer"/></typeparam>
        /// <typeparam name="TResolver">A class that implements <see cref="IMessageTypeResolver"/></typeparam>
        public static IProducerMiddlewareConfigurationBuilder AddSerializer<TSerializer, TResolver>(this IProducerMiddlewareConfigurationBuilder producer)
            where TSerializer : class, IMessageSerializer
            where TResolver : class, IMessageTypeResolver
        {
            return producer.AddSerializer(
                resolver => resolver.Resolve<TSerializer>(),
                resolver => resolver.Resolve<TResolver>());
        }

        /// <summary>
        /// Register a middleware to serialize messages
        /// </summary>
        /// <typeparam name="TSerializer">A class that implements <see cref="IMessageSerializer"/></typeparam>
        /// <typeparam name="TResolver">A class that implements <see cref="IMessageTypeResolver"/></typeparam>
        /// <param name="middlewares"></param>
        /// <param name="serializerFactory">A factory to create a <see cref="IMessageSerializer"/></param>
        /// <param name="resolverFactory">A factory to create a <see cref="IMessageTypeResolver"/></param>
        public static IProducerMiddlewareConfigurationBuilder AddSerializer<TSerializer, TResolver>(
            this IProducerMiddlewareConfigurationBuilder middlewares,
            Factory<TSerializer> serializerFactory,
            Factory<TResolver> resolverFactory)
            where TSerializer : class, IMessageSerializer
            where TResolver : class, IMessageTypeResolver
        {
            middlewares.DependencyConfigurator.AddSingleton<IMessageSerializer, TSerializer>();
            middlewares.DependencyConfigurator.AddSingleton<IMessageTypeResolver, TResolver>();
            middlewares.DependencyConfigurator.AddSingleton<TSerializer>();
            middlewares.DependencyConfigurator.AddSingleton<TResolver>();

            return middlewares.Add(
                provider => new SerializerProducerMiddleware(
                    serializerFactory(provider),
                    resolverFactory(provider)));
        }
    }
}
