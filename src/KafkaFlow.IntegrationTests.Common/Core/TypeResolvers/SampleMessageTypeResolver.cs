namespace KafkaFlow.IntegrationTests.Common.Core.TypeResolvers
{
    using System;
    using System.Text;
    using KafkaFlow.Serializer;

    public class SampleMessageTypeResolver : IMessageTypeResolver
    {
        public Type OnConsume(IMessageContext context)
        {
            var typeName = context.Headers.GetString("Message-Type");

            return Type.GetType(typeName);
        }

        public void OnProduce(IMessageContext context)
        {
            var messageTypeName = context.Message.GetType().FullName;

            context.Headers.Add("Message-Type", Encoding.UTF8.GetBytes(messageTypeName));
        }
    }
}
