namespace KafkaFlow.Serializer
{
    using System.Threading.Tasks;
    using Microsoft.IO;

    /// <summary>
    /// Middleware to serialize messages when producing
    /// </summary>
    public class SerializerProducerMiddleware : IMessageMiddleware
    {
        private readonly IMessageSerializer serializer;
        private readonly IMessageTypeResolver typeResolver;

        private static readonly RecyclableMemoryStreamManager MemoryStreamManager =
            new RecyclableMemoryStreamManager(
                1024 * 4,
                1024 * 1024 * 4,
                1024 * 1024 * 16,
                false);

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

            this.SerializeMessage(context);

            return next(context);
        }

        private void SerializeMessage(IMessageContext context)
        {
            using var stream = MemoryStreamManager.GetStream();

            var serializationContext = new SerializationContext();
            this.serializer.Serialize(context.Message, stream, serializationContext);

            context.TransformMessage(stream.ToArray());
        }
    }
}
