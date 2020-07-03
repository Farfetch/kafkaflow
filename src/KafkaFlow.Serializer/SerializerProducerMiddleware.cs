namespace KafkaFlow.Serializer
{
    using System.Threading.Tasks;

    /// <summary>
    /// Middleware to serialize messages on producers
    /// </summary>
    public class SerializerProducerMiddleware : IMessageMiddleware
    {
        private readonly IMessageSerializer serializer;
        private readonly IMessageTypeResolver typeResolver;

        /// <summary>
        /// SerializerProducerMiddleware constructor
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
        /// <param name="context">Instance of <see cref="IMessageContext"/></param>
        /// <param name="next">Next middleware to be executed</param>
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
