namespace KafkaFlow
{
    using Confluent.SchemaRegistry;
    using KafkaFlow.Configuration;
    using KafkaFlow.Serializer;
    using KafkaFlow.Serializer.SchemaRegistry;

    /// <summary>
    /// No needed
    /// </summary>
    public static class ConsumerConfigurationBuilderExtensions
    {
        /// <summary>
        /// Registers a middleware to deserialize avro messages using schema registry
        /// </summary>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <returns></returns>
        public static IConsumerMiddlewareConfigurationBuilder AddSchemaRegistryAvroSerializer(
            this IConsumerMiddlewareConfigurationBuilder middlewares)
        {
            return middlewares.Add(
                resolver => new SerializerConsumerMiddleware(
                    new ConfluentAvroSerializer(resolver),
                    new SchemaRegistryTypeResolver(new ConfluentAvroTypeNameResolver(resolver.Resolve<ISchemaRegistryClient>()))));
        }
    }
}
