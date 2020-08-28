namespace KafkaFlow.Serializer
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
        /// <typeparam name="TSerializer">A class that implements <see cref="IMessageSerializer"/></typeparam>
        /// <typeparam name="TResolver">A class that implements <see cref="IMessageTypeResolver"/></typeparam>
        public static IConsumerMiddlewareConfigurationBuilder AddSerializer<TSerializer, TResolver>(
            this IConsumerMiddlewareConfigurationBuilder consumer)
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
        /// Registers a middleware to deserialize messages
        /// </summary>
        /// <typeparam name="TSerializer">A class that implements <see cref="IMessageSerializer"/></typeparam>
        public static IConsumerMiddlewareConfigurationBuilder AddSerializer<TSerializer>(
            this IConsumerMiddlewareConfigurationBuilder consumer)
            where TSerializer : class, IMessageSerializer
        {
            return consumer.AddSerializer(
                resolver => resolver.Resolve<TSerializer>(),
                resolver => resolver.Resolve<DefaultMessageTypeResolver>());
        }

        /// <summary>
        /// Register a middleware to deserialize messages
        /// </summary>
        /// <typeparam name="TSerializer">A class that implements <see cref="IMessageSerializer"/></typeparam>
        /// <param name="consumer"></param>
        /// <param name="serializerFactory">A factory to create a <see cref="IMessageSerializer"/></param>
        public static IConsumerMiddlewareConfigurationBuilder AddSerializer<TSerializer>(
            this IConsumerMiddlewareConfigurationBuilder consumer,
            Factory<TSerializer> serializerFactory)
            where TSerializer : class, IMessageSerializer
        {
            return consumer.AddSerializer(
                serializerFactory,
                resolver => resolver.Resolve<DefaultMessageTypeResolver>());
        }

        /// <summary>
        /// Registers a middleware to serialize messages
        /// </summary>
        /// <typeparam name="TSerializer">A class that implements <see cref="IMessageSerializer"/></typeparam>
        /// <typeparam name="TResolver">A class that implements <see cref="IMessageTypeResolver"/></typeparam>
        public static IProducerMiddlewareConfigurationBuilder AddSerializer<TSerializer, TResolver>(
            this IProducerMiddlewareConfigurationBuilder producer)
            where TSerializer : class, IMessageSerializer
            where TResolver : class, IMessageTypeResolver
        {
            return producer.AddSerializer(
                resolver => resolver.Resolve<TSerializer>(),
                resolver => resolver.Resolve<TResolver>());
        }

        /// <summary>
        /// Registers a middleware to serialize messages
        /// </summary>
        /// <typeparam name="TSerializer">A class that implements <see cref="IMessageSerializer"/></typeparam>
        /// <typeparam name="TResolver">A class that implements <see cref="IMessageTypeResolver"/></typeparam>
        /// <param name="producer"></param>
        /// <param name="serializerFactory">A factory to create a <see cref="IMessageSerializer"/></param>
        /// <param name="resolverFactory">A factory to create a <see cref="IMessageTypeResolver"/></param>
        public static IProducerMiddlewareConfigurationBuilder AddSerializer<TSerializer, TResolver>(
            this IProducerMiddlewareConfigurationBuilder producer,
            Factory<TSerializer> serializerFactory,
            Factory<TResolver> resolverFactory)
            where TSerializer : class, IMessageSerializer
            where TResolver : class, IMessageTypeResolver
        {
            producer.DependencyConfigurator.AddSingleton<IMessageSerializer, TSerializer>();
            producer.DependencyConfigurator.AddSingleton<IMessageTypeResolver, TResolver>();
            producer.DependencyConfigurator.AddSingleton<TSerializer>();
            producer.DependencyConfigurator.AddSingleton<TResolver>();

            return producer.Add(
                provider => new SerializerProducerMiddleware(
                    serializerFactory(provider),
                    resolverFactory(provider)));
        }

        /// <summary>
        /// Registers a middleware to serialize messages
        /// </summary>
        /// <typeparam name="TSerializer">A class that implements <see cref="IMessageSerializer"/></typeparam>x
        public static IProducerMiddlewareConfigurationBuilder AddSerializer<TSerializer>(
            this IProducerMiddlewareConfigurationBuilder producer)
            where TSerializer : class, IMessageSerializer
        {
            return producer.AddSerializer(
                resolver => resolver.Resolve<TSerializer>(),
                resolver => resolver.Resolve<DefaultMessageTypeResolver>());
        }

        /// <summary>
        /// Registers a middleware to serialize messages
        /// </summary>
        /// <typeparam name="TSerializer">A class that implements <see cref="IMessageSerializer"/></typeparam>
        /// <param name="producer"></param>
        /// <param name="serializerFactory">A factory to create a <see cref="IMessageSerializer"/></param>
        public static IProducerMiddlewareConfigurationBuilder AddSerializer<TSerializer>(
            this IProducerMiddlewareConfigurationBuilder producer,
            Factory<TSerializer> serializerFactory)
            where TSerializer : class, IMessageSerializer
        {
            return producer.AddSerializer(
                serializerFactory,
                resolver => resolver.Resolve<DefaultMessageTypeResolver>());
        }

        /// <summary>
        /// Register a middleware to deserialize the message to a fixed type
        /// </summary>
        /// <param name="consumer"></param>
        /// <param name="serializerFactory">A factory to create a <see cref="IMessageSerializer"/></param>
        /// <typeparam name="TMessage">The message type</typeparam>
        /// <typeparam name="TSerializer">A class that implements <see cref="IMessageSerializer"/></typeparam>
        /// <returns></returns>
        public static IConsumerMiddlewareConfigurationBuilder AddSingleTypeSerializer<TMessage, TSerializer>(
            this IConsumerMiddlewareConfigurationBuilder consumer,
            Factory<TSerializer> serializerFactory)
            where TSerializer : class, IMessageSerializer
        {
            return consumer.AddSerializer(
                serializerFactory,
                resolver => resolver.Resolve<SingleMessageTypeResolver<TMessage>>());
        }

        /// <summary>
        /// Register a middleware to deserialize the message to a fixed type
        /// </summary>
        /// <param name="consumer"></param>
        /// <typeparam name="TSerializer">A class that implements <see cref="IMessageSerializer"/></typeparam>
        /// <typeparam name="TMessage">The message type</typeparam>
        /// <returns></returns>
        public static IConsumerMiddlewareConfigurationBuilder AddSingleTypeSerializer<TMessage, TSerializer>(
            this IConsumerMiddlewareConfigurationBuilder consumer)
            where TSerializer : class, IMessageSerializer
        {
            return consumer.AddSerializer<TSerializer, SingleMessageTypeResolver<TMessage>>();
        }

        /// <summary>
        /// Register a middleware to serialize the message to a fixed type
        /// </summary>
        /// <param name="producer"></param>
        /// <param name="serializerFactory">A factory to create a <see cref="IMessageSerializer"/></param>
        /// <typeparam name="TMessage">The message type</typeparam>
        /// <typeparam name="TSerializer">A class that implements <see cref="IMessageSerializer"/></typeparam>
        /// <returns></returns>
        public static IProducerMiddlewareConfigurationBuilder AddSingleTypeSerializer<TMessage, TSerializer>(
            this IProducerMiddlewareConfigurationBuilder producer,
            Factory<TSerializer> serializerFactory)
            where TSerializer : class, IMessageSerializer
        {
            return producer.AddSerializer(
                serializerFactory,
                resolver => resolver.Resolve<SingleMessageTypeResolver<TMessage>>());
        }

        /// <summary>
        /// Register a middleware to serialize the message to a fixed type
        /// </summary>
        /// <param name="producer"></param>
        /// <typeparam name="TSerializer">A class that implements <see cref="IMessageSerializer"/></typeparam>
        /// <typeparam name="TMessage">The message type</typeparam>
        /// <returns></returns>
        public static IProducerMiddlewareConfigurationBuilder AddSingleTypeSerializer<TMessage, TSerializer>(
            this IProducerMiddlewareConfigurationBuilder producer)
            where TSerializer : class, IMessageSerializer
        {
            return producer.AddSerializer<TSerializer, SingleMessageTypeResolver<TMessage>>();
        }
    }
}
