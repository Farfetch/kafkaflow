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
        /// Initializes a new instance of the <see cref="CompressorProducerMiddleware"/> class.
        /// </summary>
        /// <param name="compressor">Instance of <see cref="IMessageCompressor"/></param>
        public CompressorProducerMiddleware(IMessageCompressor compressor)
        {
            this.compressor = compressor;
        }

        /// <inheritdoc />
        public Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            if (!(context.Message is byte[] rawData))
            {
                throw new InvalidOperationException(
                    $"{nameof(context.Message)} must be a byte array to be compressed and it is '{context.Message.GetType().FullName}'");
            }

            var data = this.compressor.Compress(rawData);
            context.TransformMessage(data);

            return next(context);
        }
    }
}
