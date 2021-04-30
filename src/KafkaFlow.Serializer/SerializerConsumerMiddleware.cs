namespace KafkaFlow
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Middleware to deserialize messages when consuming
    /// </summary>
    public class SerializerConsumerMiddleware : IMessageMiddleware
    {
        private readonly ISerializer serializer;
        private readonly IMessageTypeResolver typeResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializerConsumerMiddleware"/> class.
        /// </summary>
        /// <param name="serializer">Instance of <see cref="ISerializer"/></param>
        /// <param name="typeResolver">Instance of <see cref="IMessageTypeResolver"/></param>
        public SerializerConsumerMiddleware(
            ISerializer serializer,
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
        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            var messageType = this.typeResolver.OnConsume(context);

            if (messageType is null)
            {
                return;
            }

            if (context.Message.Value is null)
            {
                await next(context).ConfigureAwait(false);
                return;
            }

            if (context.Message.Value is not byte[] rawData)
            {
                throw new InvalidOperationException(
                    $"{nameof(context.Message)} must be a byte array to be deserialized and it is '{context.Message.GetType().FullName}'");
            }

            using var stream = new MemoryStream(rawData);

            var data = await this.serializer
                .DeserializeAsync(
                    stream,
                    messageType,
                    SerializerContext.Empty)
                .ConfigureAwait(false);

            await next(context.TransformMessage(context.Message.Key, data)).ConfigureAwait(false);
        }
    }
}
