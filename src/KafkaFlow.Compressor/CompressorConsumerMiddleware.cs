namespace KafkaFlow.Compressor
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Middleware to decompress messages on consumers
    /// </summary>
    public class CompressorConsumerMiddleware : IMessageMiddleware
    {
        private readonly IMessageCompressor compressor;

        /// <summary>
        /// CompressorConsumerMiddleware constructor
        /// </summary>
        /// <param name="compressor">Instance of <see cref="IMessageCompressor"/></param>
        public CompressorConsumerMiddleware(IMessageCompressor compressor)
        {
            this.compressor = compressor;
        }

        /// <summary>
        /// Decompress message based on message compressor configured
        /// </summary>
        /// <param name="context">Instance of <see cref="IMessageContext"/></param>
        /// <param name="next">Next middleware to be executed</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Throw if message is not byte[]</exception>
        public Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            if (!(context.Message is byte[] rawData))
            {
                throw new InvalidOperationException($"{nameof(context.Message)} must be a byte array to be decompressed and it is '{context.Message.GetType().FullName}'");
            }

            var data = this.compressor.Decompress(rawData);
            context.TransformMessage(data);

            return next(context);
        }
    }
}
