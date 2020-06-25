namespace KafkaFlow.Compressor
{
    using System;
    using System.Threading.Tasks;

    public class CompressorConsumerMiddleware : IMessageMiddleware
    {
        private readonly IMessageCompressor compressor;

        public CompressorConsumerMiddleware(IMessageCompressor compressor)
        {
            this.compressor = compressor;
        }

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
