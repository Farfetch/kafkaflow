namespace KafkaFlow.Serializer
{
    using System;
    using System.Threading.Tasks;

    public class SerializerConsumerMiddleware : IMessageMiddleware
    {
        private readonly IMessageSerializer serializer;
        private readonly IMessageTypeResolver typeResolver;

        public SerializerConsumerMiddleware(
            IMessageSerializer serializer,
            IMessageTypeResolver typeResolver)
        {
            this.serializer = serializer;
            this.typeResolver = typeResolver;
        }

        public Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            var messageType = this.typeResolver.OnConsume(context);

            if (messageType is null)
            {
                return Task.CompletedTask;
            }
            
            if (context.Message is null)
            {
                return next(context);
            }

            if (!(context.Message is byte[] rawData))
            {
                throw new InvalidOperationException($"{nameof(context.Message)} must be a byte array to be serialized and it is '{context.Message.GetType().FullName}'");
            }

            context.TransformMessage(this.serializer.Deserialize(rawData, messageType));

            return next(context);
        }
    }
}
