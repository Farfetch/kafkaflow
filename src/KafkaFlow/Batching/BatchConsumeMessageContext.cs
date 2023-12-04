using System;
using System.Collections.Generic;

namespace KafkaFlow.Batching;

internal class BatchConsumeMessageContext : IMessageContext, IDisposable
{
    private readonly IDependencyResolverScope _batchDependencyScope;

    public BatchConsumeMessageContext(
        IConsumerContext consumer,
        IReadOnlyCollection<IMessageContext> batchMessage,
        IReadOnlyCollection<string> brokers)
    {
        this.ConsumerContext = consumer;
        this.Message = new Message(null, batchMessage);
        _batchDependencyScope = consumer.WorkerDependencyResolver.CreateScope();
        this.Items = new Dictionary<string, object>();
        this.Brokers = brokers;
    }

    public Message Message { get; }

    public IMessageHeaders Headers { get; } = new MessageHeaders();

    public IConsumerContext ConsumerContext { get; }

    public IProducerContext ProducerContext => null;

    public IDependencyResolver DependencyResolver => _batchDependencyScope.Resolver;

    public IDictionary<string, object> Items { get; }

    public IReadOnlyCollection<string> Brokers { get; }

    public IMessageContext SetMessage(object key, object value) =>
        throw new NotSupportedException($"{nameof(BatchConsumeMessageContext)} does not allow to change the message");

    public void Dispose() => _batchDependencyScope.Dispose();
}
