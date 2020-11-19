namespace KafkaFlow.Serializer
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.IO;

    /// <summary>
    /// Middleware to deserialize messages when consuming
    /// </summary>
    public class SerializerConsumerMiddleware : IMessageMiddleware
    {
        private readonly IMessageSerializer serializer;
        private readonly IMessageTypeResolver typeResolver;

        /// <summary>
        /// Creates a <see cref="SerializerConsumerMiddleware"/> instance
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
        /// Deserializes the message using the passed serialized
        /// </summary>
        /// <param name="context">The <see cref="IMessageContext"/> containing the message and metadata</param>
        /// <param name="next">A delegate to the next middleware</param>
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

            using var stream = new MemoryStream(rawData);
            var message = this.serializer.Deserialize(stream, messageType, new SerializationContext());
            
            context.TransformMessage(message);

            return next(context);
        }
    }
}
