using System.Threading.Tasks;
using KafkaFlow.Middlewares.Serializer.Resolvers;
using Microsoft.IO;

namespace KafkaFlow.Middlewares.Serializer
{
    /// <summary>
    /// Middleware to serialize messages when producing
    /// </summary>
    public class SerializerProducerMiddleware : IMessageMiddleware
    {
        private static readonly RecyclableMemoryStreamManager s_memoryStreamManager = new();

        private readonly ISerializer _serializer;

        private readonly IMessageTypeResolver _typeResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializerProducerMiddleware"/> class.
        /// </summary>
        /// <param name="serializer">Instance of <see cref="ISerializer"/></param>
        /// <param name="typeResolver">Instance of <see cref="IMessageTypeResolver"/></param>
        public SerializerProducerMiddleware(
            ISerializer serializer,
            IMessageTypeResolver typeResolver)
        {
            _serializer = serializer;
            _typeResolver = typeResolver;
        }

        /// <summary>
        /// Serializes message based on message type resolver strategy
        /// </summary>
        /// <param name="context">The <see cref="IMessageContext"/> containing the message and metadata</param>
        /// <param name="next">A delegate to call next middleware</param>
        /// <returns></returns>
        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            await _typeResolver.OnProduceAsync(context);

            byte[] messageValue;

            using (var buffer = s_memoryStreamManager.GetStream())
            {
                await _serializer
                    .SerializeAsync(
                        context.Message.Value,
                        buffer,
                        new SerializerContext(context.ProducerContext.Topic))
                    .ConfigureAwait(false);

                messageValue = buffer.ToArray();
            }

            await next(context.SetMessage(context.Message.Key, messageValue)).ConfigureAwait(false);
        }
    }
}
