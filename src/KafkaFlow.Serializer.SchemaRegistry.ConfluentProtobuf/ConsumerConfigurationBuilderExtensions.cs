namespace KafkaFlow
{
    using Confluent.SchemaRegistry;
    using KafkaFlow.Configuration;
    using KafkaFlow.Middlewares.Serializer;
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
        public static IConsumerMiddlewareConfigurationBuilder AddSchemaRegistryProtobufDeserializer(
            this IConsumerMiddlewareConfigurationBuilder middlewares)
        {
            return middlewares.Add(
                resolver => new DeserializerConsumerMiddleware(
                    new ConfluentProtobufDeserializer(),
                    new SchemaRegistryTypeResolver(new ConfluentProtobufTypeNameResolver(resolver.Resolve<ISchemaRegistryClient>()))));
        }
    }
}
