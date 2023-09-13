namespace KafkaFlow
{
    internal class MessageContext : IMessageContext
    {
        public MessageContext(
            Message message,
            IMessageHeaders headers,
            IDependencyResolver dependencyResolver,
            IConsumerContext consumer,
            IProducerContext producer)
        {
            this.Message = message;
            this.DependencyResolver = dependencyResolver;
            this.Headers = headers ?? new MessageHeaders();
            this.ConsumerContext = consumer;
            this.ProducerContext = producer;
        }

        public Message Message { get; }

        public IDependencyResolver DependencyResolver { get; }

        public IConsumerContext ConsumerContext { get; }

        public IProducerContext ProducerContext { get; }

        public IMessageHeaders Headers { get; }

        public IMessageContext SetMessage(object key, object value) => new MessageContext(
            new Message(key, value),
            this.Headers,
            this.DependencyResolver,
            this.ConsumerContext,
            this.ProducerContext);
    }
}
