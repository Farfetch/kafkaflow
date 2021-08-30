namespace KafkaFlow
{
    using Confluent.SchemaRegistry;
    using Confluent.SchemaRegistry.Serdes;
    using KafkaFlow.Configuration;
    using KafkaFlow.Serializer.SchemaRegistry;

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
            return middlewares.AddSerializer(
                resolver => new ConfluentProtobufSerializer(resolver, config),
                resolver => new SchemaRegistryTypeResolver(new ConfluentProtobufTypeNameResolver(resolver.Resolve<ISchemaRegistryClient>())));
        }
    }
}
