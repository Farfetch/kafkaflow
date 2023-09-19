namespace KafkaFlow.Middlewares.Serializer.Resolvers
{
    using System;
    using System.Threading.Tasks;

    internal class DefaultTypeResolver : IMessageTypeResolver
    {
        private const string MessageType = "Message-Type";

        public ValueTask<Type> OnConsumeAsync(IMessageContext context)
        {
            var typeName = context.Headers.GetString(MessageType);

            return typeName is null ?
                new ValueTask<Type>((Type) null) :
                new ValueTask<Type>(Type.GetType(typeName));
        }

        public ValueTask OnProduceAsync(IMessageContext context)
        {
            if (context.Message.Value is null)
            {
                return default(ValueTask);
            }

            var messageType = context.Message.Value.GetType();

            context.Headers.SetString(
                MessageType,
                $"{messageType.FullName}, {messageType.Assembly.GetName().Name}");

            return default(ValueTask);
        }
    }
}
