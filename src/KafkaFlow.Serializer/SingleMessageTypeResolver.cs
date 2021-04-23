namespace KafkaFlow
{
    using System;

    internal class SingleMessageTypeResolver<TMessage> : IMessageTypeResolver
    {
        public Type OnConsume(IMessageContext context) => typeof(TMessage);

        public void OnProduce(IMessageContext context)
        {
            // Nothing to do when producing
        }
    }
}
