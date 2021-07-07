namespace KafkaFlow
{
    using System;

    internal class SingleMessageTypeResolver : IMessageTypeResolver
    {
        private readonly Type messageType;

        public SingleMessageTypeResolver(Type messageType)
        {
            this.messageType = messageType;
        }

        public Type OnConsume(IMessageContext context) => this.messageType;

        public void OnProduce(IMessageContext context)
        {
            // Nothing to do when producing
        }
    }
}
