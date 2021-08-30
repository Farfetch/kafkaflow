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
        /// Registers a middleware to serialize avro messages using schema registry
        /// </summary>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <param name="config">The avro serializer configuration</param>
        /// <returns></returns>
        public static IProducerMiddlewareConfigurationBuilder AddSchemaRegistryAvroSerializer(
            this IProducerMiddlewareConfigurationBuilder middlewares,
            AvroSerializerConfig config = null)
        {
            return middlewares.AddSerializer(
                resolver => new ConfluentAvroSerializer(resolver, config),
                resolver => new SchemaRegistryTypeResolver(new ConfluentAvroTypeNameResolver(resolver.Resolve<ISchemaRegistryClient>())));
        }
    }
}
