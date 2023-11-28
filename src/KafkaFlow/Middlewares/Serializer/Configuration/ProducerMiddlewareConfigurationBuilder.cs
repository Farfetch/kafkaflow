using System;
using KafkaFlow.Configuration;
using KafkaFlow.Middlewares.Serializer;
using KafkaFlow.Middlewares.Serializer.Resolvers;

namespace KafkaFlow
{
    /// <summary>
    /// No needed
    /// </summary>
    public static class ProducerMiddlewareConfigurationBuilder
    {
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

            return middlewares.AddSerializer(
                resolver => resolver.Resolve<TSerializer>(),
                resolver => resolver.Resolve<TResolver>());
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

            return middlewares.AddSerializer(
                resolver => resolver.Resolve<TSerializer>(),
                _ => new DefaultTypeResolver());
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
                _ => new DefaultTypeResolver());
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
                _ => new SingleMessageTypeResolver(typeof(TMessage)));
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
            return middlewares.AddSingleTypeSerializer<TSerializer>(typeof(TMessage));
        }

        /// <summary>
        /// Register a middleware to serialize the message to a fixed type
        /// </summary>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <param name="messageType">The message type</param>
        /// <typeparam name="TSerializer">A class that implements <see cref="ISerializer"/></typeparam>
        /// <returns></returns>
        public static IProducerMiddlewareConfigurationBuilder AddSingleTypeSerializer<TSerializer>(
            this IProducerMiddlewareConfigurationBuilder middlewares,
            Type messageType)
            where TSerializer : class, ISerializer
        {
            middlewares.DependencyConfigurator.AddTransient<TSerializer>();

            return middlewares.AddSerializer(
                resolver => resolver.Resolve<TSerializer>(),
                _ => new SingleMessageTypeResolver(messageType));
        }

        /// <summary>
        /// Register a middleware to serialize the message to a fixed type
        /// </summary>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <param name="serializerFactory">A factory to create a <see cref="ISerializer"/></param>
        /// <param name="messageType">The message type</param>
        /// <typeparam name="TSerializer">A class that implements <see cref="ISerializer"/></typeparam>
        /// <returns></returns>
        public static IProducerMiddlewareConfigurationBuilder AddSingleTypeSerializer<TSerializer>(
            this IProducerMiddlewareConfigurationBuilder middlewares,
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
