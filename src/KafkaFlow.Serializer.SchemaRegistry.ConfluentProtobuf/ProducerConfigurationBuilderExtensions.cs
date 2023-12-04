using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using KafkaFlow.Configuration;
using KafkaFlow.Middlewares.Serializer;
using KafkaFlow.Serializer.SchemaRegistry;

namespace KafkaFlow;

/// <summary>
/// No needed
/// </summary>
public static class ProducerConfigurationBuilderExtensions
{
    /// <summary>
    /// Registers a middleware to serialize protobuf messages using schema registry
    /// </summary>
    /// <param name="middlewares">The middleware configuration builder</param>
    /// <param name="config">The json serializer configuration</param>
    /// <returns></returns>
    public static IProducerMiddlewareConfigurationBuilder AddSchemaRegistryProtobufSerializer(
        this IProducerMiddlewareConfigurationBuilder middlewares,
        ProtobufSerializerConfig config = null)
    {
        return middlewares.Add(
            resolver => new SerializerProducerMiddleware(
                new ConfluentProtobufSerializer(resolver, config),
                new SchemaRegistryTypeResolver(new ConfluentProtobufTypeNameResolver(resolver.Resolve<ISchemaRegistryClient>()))));
    }
}
