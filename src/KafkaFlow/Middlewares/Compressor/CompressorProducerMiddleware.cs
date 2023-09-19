namespace KafkaFlow.Middlewares.Compressor
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Middleware to compress the messages when producing
    /// </summary>
    public class CompressorProducerMiddleware : IMessageMiddleware
    {
        private readonly ICompressor compressor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompressorProducerMiddleware"/> class.
        /// </summary>
        /// <param name="compressor">Instance of <see cref="ICompressor"/></param>
        public CompressorProducerMiddleware(ICompressor compressor)
        {
            this.compressor = compressor;
        }

        /// <inheritdoc />
        public Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            if (context.Message.Value is not byte[] rawData)
            {
                throw new InvalidOperationException(
                    $"{nameof(context.Message.Value)} must be a byte array to be compressed and it is '{context.Message.Value.GetType().FullName}'");
            }

            var data = this.compressor.Compress(rawData);

            return next(context.SetMessage(context.Message.Key, data));
        }
    }
}
