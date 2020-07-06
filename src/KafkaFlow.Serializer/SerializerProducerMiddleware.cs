namespace KafkaFlow.Serializer
{
    using System.Threading.Tasks;

    /// <summary>
    /// Middleware to serialize messages when producing
    /// </summary>
    public class SerializerProducerMiddleware : IMessageMiddleware
    {
        private readonly IMessageSerializer serializer;
        private readonly IMessageTypeResolver typeResolver;

        /// <summary>
        /// creates a <see cref="SerializerProducerMiddleware"/> instance
        /// </summary>
        /// <param name="serializer">Instance of <see cref="IMessageSerializer"/></param>
        /// <param name="typeResolver">Instance of <see cref="IMessageTypeResolver"/></param>
        public SerializerProducerMiddleware(
            IMessageSerializer serializer,
            IMessageTypeResolver typeResolver)
        {
            this.serializer = serializer;
            this.typeResolver = typeResolver;
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

            var data = this.serializer.Serialize(context.Message);
            
            context.TransformMessage(data);

            return next(context);
        }
    }
}
