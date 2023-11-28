using KafkaFlow.Configuration;
using KafkaFlow.Middlewares.Serializer;
using KafkaFlow.Serializer.SchemaRegistry;

namespace KafkaFlow
{
    /// <summary>
    /// No needed
    /// </summary>
    public static class ConsumerConfigurationBuilderExtensions
    {
        /// <summary>
        /// Registers a middleware to deserialize protobuf messages using schema registry
        /// </summary>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <returns></returns>
        public static IConsumerMiddlewareConfigurationBuilder AddSchemaRegistryProtobufDeserializer(
            this IConsumerMiddlewareConfigurationBuilder middlewares)
        {
            middlewares.DependencyConfigurator.TryAddTransient<IConfluentProtobufTypeNameResolver, ConfluentProtobufTypeNameResolver>();

            return middlewares.Add(
                resolver => new DeserializerConsumerMiddleware(
                    new ConfluentProtobufDeserializer(),
                    new SchemaRegistryTypeResolver(resolver.Resolve<IConfluentProtobufTypeNameResolver>())));
        }
    }
}
