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
            this.Items = new Dictionary<string, object>();
        }

        public Message Message { get; }

        public IMessageHeaders Headers { get; } = new MessageHeaders();

        public IConsumerContext ConsumerContext { get; }

        public IProducerContext ProducerContext => null;

        public IDependencyResolver DependencyResolver => this.batchDependencyScope.Resolver;

        public IDictionary<string, object> Items { get; }

        public IMessageContext SetMessage(object key, object value) =>
            throw new NotSupportedException($"{nameof(BatchConsumeMessageContext)} does not allow to change the message");

        public void Dispose() => this.batchDependencyScope.Dispose();
    }
}
