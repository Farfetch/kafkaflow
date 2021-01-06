namespace KafkaFlow.Serializer
{
    using System.Threading.Tasks;
    using Configuration;

    /// <summary>
    /// Middleware to serialize messages when producing
    /// </summary>
    public class SerializerProducerMiddleware : IMessageMiddleware
    {
        private readonly IMessageSerializer serializer;
        private readonly IMessageTypeResolver typeResolver;
        private readonly SchemaRegistryConfiguration schemaRegistryConfiguration;

        /// <summary>
        /// creates a <see cref="SerializerProducerMiddleware"/> instance
        /// </summary>
        /// <param name="serializer">Instance of <see cref="IMessageSerializer"/></param>
        /// <param name="typeResolver">Instance of <see cref="IMessageTypeResolver"/></param>
        public SerializerProducerMiddleware(
            IMessageSerializer serializer,
            IMessageTypeResolver typeResolver,
            SchemaRegistryConfiguration schemaRegistryConfiguration)
        {
            this.serializer = serializer;
            this.typeResolver = typeResolver;
            this.schemaRegistryConfiguration = schemaRegistryConfiguration;
        }

        /// <summary>
        /// Serializes message based on message type resolver strategy
        /// </summary>
        /// <param name="context">The <see cref="IMessageContext"/> containing the message and metadata</param>
        /// <param name="next">A delegate to call next middleware</param>
        /// <returns></returns>
        public Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            this.typeResolver.OnProduce(context);

            var data = this.serializer.Serialize(context.Message, schemaRegistryConfiguration);
            
            context.TransformMessage(data);

            return next(context);
        }
    }
}
