namespace KafkaFlow.BatchConsume
{
    using System;
    using System.Collections.Generic;

    internal class BatchConsumeMessageContext : IMessageContext, IDisposable
    {
        private readonly IDependencyResolverScope batchDependencyScope;

        public BatchConsumeMessageContext(
            IConsumerContext consumer,
            IReadOnlyCollection<IMessageContext> batchMessage)
        {
            this.ConsumerContext = consumer;
            this.Message = new Message(null, batchMessage);
            this.batchDependencyScope = consumer.WorkerDependencyResolver.CreateScope();
        }

        public Message Message { get; }

        public IMessageHeaders Headers { get; } = new MessageHeaders();

        public IConsumerContext ConsumerContext { get; }

        public IProducerContext ProducerContext => null;

        public IDependencyResolver DependencyResolver => this.batchDependencyScope.Resolver;

        public IMessageContext SetMessage(object key, object value) =>
            throw new NotSupportedException($"{nameof(BatchConsumeMessageContext)} does not allow change the message");

        public IMessageContext TransformMessage(object message) => throw new NotImplementedException();

        public void Dispose() => this.batchDependencyScope.Dispose();
    }
}
