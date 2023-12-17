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
    /// Registers a middleware to serialize avro messages using schema registry
    /// </summary>
    /// <param name="middlewares">The middleware configuration builder</param>
    /// <param name="config">The avro serializer configuration</param>
    /// <returns></returns>
    public static IProducerMiddlewareConfigurationBuilder AddSchemaRegistryAvroConvertSerializer(
        this IProducerMiddlewareConfigurationBuilder middlewares,
        AvroSerializerConfig config = null)
    {
        return middlewares.Add(
            resolver => new SerializerProducerMiddleware(
                new AvroConvertSerializer(resolver, config),
                new SchemaRegistryTypeResolver(new ConfluentAvroTypeNameResolver(resolver.Resolve<ISchemaRegistryClient>()))));
    }
}
