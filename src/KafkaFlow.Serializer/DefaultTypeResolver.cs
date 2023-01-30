namespace KafkaFlow
{
    using System;

    internal class DefaultTypeResolver : IMessageTypeResolver
    {
        private const string MessageType = "Message-Type";

        public Type OnConsume(IMessageContext context)
        {
            var typeName = context.Headers.GetString(MessageType);

            return typeName is null ?
                null :
                Type.GetType(typeName);
        }

        public void OnProduce(IMessageContext context)
        {
            if (context.Message.Value is null)
            {
                return;
            }

            var messageType = context.Message.Value.GetType();

            context.Headers.SetString(
                MessageType,
                $"{messageType.FullName}, {messageType.Assembly.GetName().Name}");
        }
    }
}
