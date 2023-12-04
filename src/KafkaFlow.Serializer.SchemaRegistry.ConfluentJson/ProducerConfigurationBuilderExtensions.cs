using Confluent.SchemaRegistry.Serdes;
using KafkaFlow.Configuration;
using KafkaFlow.Middlewares.Serializer.Resolvers;
using KafkaFlow.Serializer.SchemaRegistry;

namespace KafkaFlow;

/// <summary>
/// No needed
/// </summary>
public static class ProducerConfigurationBuilderExtensions
{
    /// <summary>
    /// Registers a middleware to serialize json messages using schema registry
    /// </summary>
    /// <param name="middlewares">The middleware configuration builder</param>
    /// <param name="config">The protobuf serializer configuration</param>
    /// <typeparam name="TMessage">The message type</typeparam>
    /// <returns></returns>
    public static IProducerMiddlewareConfigurationBuilder AddSchemaRegistryJsonSerializer<TMessage>(
        this IProducerMiddlewareConfigurationBuilder middlewares,
        JsonSerializerConfig config = null)
    {
        return middlewares.AddSerializer(
            resolver => new ConfluentJsonSerializer(resolver, config),
            _ => new SingleMessageTypeResolver(typeof(TMessage)));
    }
}
