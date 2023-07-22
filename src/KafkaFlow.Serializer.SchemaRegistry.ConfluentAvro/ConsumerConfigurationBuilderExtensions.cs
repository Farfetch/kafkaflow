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
        /// Registers a middleware to deserialize avro messages using schema registry
        /// </summary>
        /// <param name="middlewares">The middleware configuration builder</param>
        /// <returns></returns>
        public static IConsumerMiddlewareConfigurationBuilder AddSchemaRegistryAvroSerializer(
            this IConsumerMiddlewareConfigurationBuilder middlewares)
        {
            middlewares.DependencyConfigurator.TryAddTransient<IConfluentAvroTypeNameResolver, ConfluentAvroTypeNameResolver>();

            return middlewares.Add(
                resolver => new SerializerConsumerMiddleware(
                    new ConfluentAvroSerializer(resolver),
                    new SchemaRegistryTypeResolver(resolver.Resolve<IConfluentAvroTypeNameResolver>())));
        }
    }
}
