namespace KafkaFlow.Compressor
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Middleware to compress the messages when producing
    /// </summary>
    internal class CompressorProducerMiddleware : IMessageMiddleware
    {
        private readonly IMessageCompressor compressor;

        /// <summary>
        /// Creates a <see cref="CompressorProducerMiddleware"/> instance
        /// </summary>
        /// <param name="compressor">Instance of <see cref="IMessageCompressor"/></param>
        public CompressorProducerMiddleware(IMessageCompressor compressor)
        {
            this.compressor = compressor;
        }

        /// <summary>
        /// Compress message based on the passed compressor.
        /// </summary>
        /// <param name="context">Instance of <see cref="IMessageContext"/></param>
        /// <param name="next">Next middleware to be executed</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Throw if message is not byte[]</exception>
        public Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            if (!(context.Message is byte[] rawData))
            {
                throw new InvalidOperationException($"{nameof(context.Message)} must be a byte array to be compressed and it is '{context.Message.GetType().FullName}'");
            }

            var data = this.compressor.Compress(rawData);
            context.TransformMessage(data);

            return next(context);
        }
    }
}
