using KafkaFlow.Configuration;
using KafkaFlow.Middlewares.Serializer.Resolvers;
using KafkaFlow.Serializer.SchemaRegistry;

namespace KafkaFlow;

/// <summary>
/// No needed
/// </summary>
public static class ConsumerConfigurationBuilderExtensions
{
    /// <summary>
    /// Registers a middleware to deserialize json messages using schema registry
    /// </summary>
    /// <param name="middlewares">The middleware configuration builder</param>
    /// <typeparam name="TMessage">The message type</typeparam>
    /// <returns></returns>
    public static IConsumerMiddlewareConfigurationBuilder AddSchemaRegistryJsonSerializer<TMessage>(
        this IConsumerMiddlewareConfigurationBuilder middlewares)
    {
        return middlewares.AddDeserializer(
            resolver => new ConfluentJsonDeserializer(),
            _ => new SingleMessageTypeResolver(typeof(TMessage)));
    }
}
