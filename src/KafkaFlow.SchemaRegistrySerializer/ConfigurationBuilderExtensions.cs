namespace KafkaFlow
{
    using System;
    using System.Linq;
    using Confluent.SchemaRegistry;
    using KafkaFlow.Configuration;
    using KafkaFlow.SchemaRegistrySerializer;

    /// <summary>
    /// Extension methods for <see cref="IConsumerMiddlewareConfigurationBuilder"/> and <see cref="IProducerMiddlewareConfigurationBuilder"/>
    /// </summary>
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// Configure Schema Registry functionality
        /// </summary>
        /// <param name="kafka"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IKafkaConfigurationBuilder WithSchemaRegistry(
            this IKafkaConfigurationBuilder kafka,
            SchemaRegistryConfig config)
        {
            kafka.DependencyConfigurator
                .AddSingleton<ISchemaResolver, SchemaResolver>()
                .AddSingleton<ISchemaRegistryManager>(
                    resolver => new SchemaRegistryManager(new CachedSchemaRegistryClient(config)))
                .AddSingleton<IMessageTypeFinder>(
                    new MessageTypeFinder(
                        AppDomain.CurrentDomain
                            .GetAssemblies()
                            .SelectMany(x => x.GetTypes())));

            return kafka;
        }

        /// <summary>
        /// Registers a middleware to deserialize messages using Schema Registry
        /// </summary>
        /// <typeparam name="TSerializer">A class that implements <see cref="IMessageSerializer"/></typeparam>
        public static IConsumerMiddlewareConfigurationBuilder DeserializeWithSchemaRegistry<TSerializer>(
            this IConsumerMiddlewareConfigurationBuilder middlewares)
            where TSerializer : class, IMessageSerializer
        {
            middlewares.DependencyConfigurator.AddTransient<TSerializer>();

            return middlewares.Add(
                resolver => new SerializerConsumerMiddleware(
                    resolver.Resolve<TSerializer>(),
                    resolver.Resolve<IMessageTypeFinder>(),
                    resolver.Resolve<ISchemaRegistryManager>(),
                    resolver.Resolve<ISchemaResolver>()));
        }

        /// <summary>
        /// Register a middleware to deserialize messages using Schema Registry
        /// </summary>
        /// <typeparam name="TSerializer">A class that implements <see cref="IMessageSerializer"/></typeparam>
        /// <param name="middlewares"></param>
        /// <param name="serializerFactory">A factory to create a <see cref="IMessageSerializer"/></param>
        public static IConsumerMiddlewareConfigurationBuilder DeserializeWithSchemaRegistry<TSerializer>(
            this IConsumerMiddlewareConfigurationBuilder middlewares,
            Factory<TSerializer> serializerFactory)
            where TSerializer : class, IMessageSerializer
        {
            return middlewares.Add(
                resolver => new SerializerConsumerMiddleware(
                    serializerFactory(resolver),
                    resolver.Resolve<IMessageTypeFinder>(),
                    resolver.Resolve<ISchemaRegistryManager>(),
                    resolver.Resolve<ISchemaResolver>()));
        }

        /// <summary>
        /// Registers a middleware to serialize messages using Schema Registry
        /// </summary>
        /// <typeparam name="TSerializer">A class that implements <see cref="IMessageSerializer"/></typeparam>x
        public static IProducerMiddlewareConfigurationBuilder SerializeWithSchemaRegistry<TSerializer>(
            this IProducerMiddlewareConfigurationBuilder middlewares)
            where TSerializer : class, IMessageSerializer
        {
            middlewares.DependencyConfigurator.AddTransient<TSerializer>();

            return middlewares.Add(
                resolver => new SerializerProducerMiddleware(
                    resolver.Resolve<TSerializer>(),
                    resolver.Resolve<ISchemaResolver>()));
        }

        /// <summary>
        /// Registers a middleware to serialize messages using Schema Registry
        /// </summary>
        /// <typeparam name="TSerializer">A class that implements <see cref="IMessageSerializer"/></typeparam>
        /// <param name="middlewares"></param>
        /// <param name="serializerFactory">A factory to create a <see cref="IMessageSerializer"/></param>
        public static IProducerMiddlewareConfigurationBuilder SerializeWithSchemaRegistry<TSerializer>(
            this IProducerMiddlewareConfigurationBuilder middlewares,
            Factory<TSerializer> serializerFactory)
            where TSerializer : class, IMessageSerializer
        {
            return middlewares.Add(
                resolver => new SerializerProducerMiddleware(
                    serializerFactory(resolver),
                    resolver.Resolve<ISchemaResolver>()));
        }
    }
}
