namespace KafkaFlow.Serializer
{
    using System;

    public interface IMessageTypeResolver
    {
        Type OnConsume(IMessageContext context);

        void OnProduce(IMessageContext context);
    }
}
