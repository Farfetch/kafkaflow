using System.Collections.Generic;

namespace KafkaFlow;

internal class MessageContext : IMessageContext
{
    public MessageContext(
        Message message,
        IMessageHeaders headers,
        IDependencyResolver dependencyResolver,
        IConsumerContext consumer,
        IProducerContext producer,
        IReadOnlyCollection<string> brokers)
    {
        this.Message = message;
        this.DependencyResolver = dependencyResolver;
        this.Headers = headers ?? new MessageHeaders();
        this.ConsumerContext = consumer;
        this.ProducerContext = producer;
        this.Items = new Dictionary<string, object>();
        this.Brokers = brokers;
    }

    public Message Message { get; }

    public IDependencyResolver DependencyResolver { get; }

    public IConsumerContext ConsumerContext { get; }

    public IProducerContext ProducerContext { get; }

    public IMessageHeaders Headers { get; }

    public IDictionary<string, object> Items { get; }

    public IReadOnlyCollection<string> Brokers { get; }

    public IMessageContext SetMessage(object key, object value) => new MessageContext(
        new Message(key, value),
        this.Headers,
        this.DependencyResolver,
        this.ConsumerContext,
        this.ProducerContext,
        this.Brokers);
}
