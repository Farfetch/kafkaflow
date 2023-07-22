namespace KafkaFlow
{
    using Confluent.SchemaRegistry;
    using KafkaFlow.Configuration;
    using KafkaFlow.Serializer.SchemaRegistry;

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
        public static IConsumerMiddlewareConfigurationBuilder AddSchemaRegistryProtobufSerializer(
            this IConsumerMiddlewareConfigurationBuilder middlewares)
        {
            middlewares.DependencyConfigurator.TryAddTransient<IConfluentProtobufTypeNameResolver, ConfluentProtobufTypeNameResolver>();

            return middlewares.Add(
                resolver => new SerializerConsumerMiddleware(
                    new ConfluentProtobufSerializer(resolver),
                    new SchemaRegistryTypeResolver(resolver.Resolve<IConfluentProtobufTypeNameResolver>())));
        }
    }
}
