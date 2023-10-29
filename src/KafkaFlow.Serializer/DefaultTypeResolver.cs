namespace KafkaFlow
{
    using System;
    using System.Collections.Concurrent;

    internal class DefaultTypeResolver : IMessageTypeResolver
    {
        private const string MessageType = "Message-Type";

        private static readonly ConcurrentDictionary<string, Type> ConsumeTypeCache = new(StringComparer.Ordinal);
        private static readonly ConcurrentDictionary<Type, string> ProduceTypeCache = new();

        public Type OnConsume(IMessageContext context)
        {
            var typeName = context.Headers.GetString(MessageType);

            return typeName is null ?
               null :
               ConsumeTypeCache.GetOrAdd(typeName, Type.GetType);
        }

        public void OnProduce(IMessageContext context)
        {
            if (context.Message.Value is null)
            {
                return;
            }

            var messageType = context.Message.Value.GetType();

            string messageTypeName = ProduceTypeCache.GetOrAdd(
                messageType,
                static messageType => $"{messageType.FullName}, {messageType.Assembly.GetName().Name}");

            context.Headers.SetString(MessageType, messageTypeName);
        }
    }
}
