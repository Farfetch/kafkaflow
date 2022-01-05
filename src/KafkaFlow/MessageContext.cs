namespace KafkaFlow
{
    internal class MessageContext : IMessageContext
    {
        public MessageContext(
            Message message,
            IMessageHeaders headers,
            string clusterName,
            IConsumerContext consumer,
            IProducerContext producer)
        {
            this.Message = message;
            this.ClusterName = clusterName;
            this.Headers = headers ?? new MessageHeaders();
            this.ConsumerContext = consumer;
            this.ProducerContext = producer;
        }

        public Message Message { get; }

        public string ClusterName { get; }

        public IConsumerContext ConsumerContext { get; }

        public IProducerContext ProducerContext { get; }

        public IMessageHeaders Headers { get; }

        public IMessageContext SetMessage(object key, object value) => new MessageContext(
            new Message(key, value),
            this.Headers,
            this.ClusterName,
            this.ConsumerContext,
            this.ProducerContext);

        public IMessageContext TransformMessage(object message) => throw new System.NotImplementedException();
    }
}
