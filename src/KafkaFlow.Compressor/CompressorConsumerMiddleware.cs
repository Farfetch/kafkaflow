namespace KafkaFlow.Compressor
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Middleware to decompress the messages when consuming
    /// </summary>
    public class CompressorConsumerMiddleware : IMessageMiddleware
    {
        private readonly IMessageCompressor compressor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompressorConsumerMiddleware"/> class.
        /// </summary>
        /// <param name="compressor">Instance of <see cref="IMessageCompressor"/></param>
        public CompressorConsumerMiddleware(IMessageCompressor compressor)
        {
            this.compressor = compressor;
        }

        /// <inheritdoc />
        public Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            if (!(context.Message.Value is byte[] rawData))
            {
                throw new InvalidOperationException(
                    $"{nameof(context.Message)} must be a byte array to be decompressed and it is '{context.Message.GetType().FullName}'");
            }

            var data = this.compressor.Decompress(rawData);

            return next(context.TransformMessage(context.Message.Key, data));
        }
    }
}
