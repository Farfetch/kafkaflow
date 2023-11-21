using System;
using System.IO;
using System.Threading.Tasks;
using KafkaFlow.Middlewares.Serializer.Resolvers;

namespace KafkaFlow.Middlewares.Serializer
{
    /// <summary>
    /// Middleware to deserialize messages when consuming
    /// </summary>
    public class DeserializerConsumerMiddleware : IMessageMiddleware
    {
        private readonly IDeserializer _deserializer;
        private readonly IMessageTypeResolver _typeResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeserializerConsumerMiddleware"/> class.
        /// </summary>
        /// <param name="deserializer">Instance of <see cref="IDeserializer"/></param>
        /// <param name="typeResolver">Instance of <see cref="IMessageTypeResolver"/></param>
        public DeserializerConsumerMiddleware(
            IDeserializer deserializer,
            IMessageTypeResolver typeResolver)
        {
            _deserializer = deserializer;
            _typeResolver = typeResolver;
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

            if (rawData.Length == 0)
            {
                await next(context.SetMessage(context.Message.Key, null)).ConfigureAwait(false);
                return;
            }

            var messageType = await _typeResolver.OnConsumeAsync(context);

            if (messageType is null)
            {
                return;
            }

            using var stream = new MemoryStream(rawData);

            var data = await _deserializer
                .DeserializeAsync(
                    stream,
                    messageType,
                    new SerializerContext(context.ConsumerContext.Topic))
                .ConfigureAwait(false);

            await next(context.SetMessage(context.Message.Key, data)).ConfigureAwait(false);
        }
    }
}
