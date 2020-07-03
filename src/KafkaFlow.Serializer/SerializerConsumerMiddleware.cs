namespace KafkaFlow.Serializer
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Middleware to deserialize messages on consumers
    /// </summary>
    public class SerializerConsumerMiddleware : IMessageMiddleware
    {
        private readonly IMessageSerializer serializer;
        private readonly IMessageTypeResolver typeResolver;

        /// <summary>
        /// SerializerConsumerMiddleware constructor
        /// </summary>
        /// <param name="serializer">Instance of <see cref="IMessageSerializer"/></param>
        /// <param name="typeResolver">Instance of <see cref="IMessageTypeResolver"/></param>
        public SerializerConsumerMiddleware(
            IMessageSerializer serializer,
            IMessageTypeResolver typeResolver)
        {
            this.serializer = serializer;
            this.typeResolver = typeResolver;
        }

        /// <summary>
        /// Deserializes message based on message type resolver and message serializer configured
        /// </summary>
        /// <param name="context">Instance of <see cref="IMessageContext"/></param>
        /// <param name="next">Next middleware to be executed</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Throw if message is not byte[]</exception>
        public Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            var messageType = this.typeResolver.OnConsume(context);

            if (messageType is null)
            {
                return Task.CompletedTask;
            }
            
            if (context.Message is null)
            {
                return next(context);
            }

            if (!(context.Message is byte[] rawData))
            {
                throw new InvalidOperationException($"{nameof(context.Message)} must be a byte array to be deserialized and it is '{context.Message.GetType().FullName}'");
            }

            context.TransformMessage(this.serializer.Deserialize(rawData, messageType));

            return next(context);
        }
    }
}
