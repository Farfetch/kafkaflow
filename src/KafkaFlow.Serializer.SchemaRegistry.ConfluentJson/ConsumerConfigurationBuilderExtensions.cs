namespace KafkaFlow
{
    using KafkaFlow.Configuration;
    using KafkaFlow.Serializer.SchemaRegistry;

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
            return middlewares.AddSerializer(
                resolver => new ConfluentJsonSerializer(resolver),
                _ => new SingleMessageTypeResolver(typeof(TMessage)));
        }
    }
}
