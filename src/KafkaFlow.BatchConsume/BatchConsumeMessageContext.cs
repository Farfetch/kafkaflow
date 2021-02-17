namespace KafkaFlow.BatchConsume
{
    using System.Collections.Generic;

    internal class BatchConsumeMessageContext : IMessageContext
    {
        public BatchConsumeMessageContext(
            int workerId,
            string groupId,
            IMessageContextConsumer consumer,
            IReadOnlyCollection<IMessageContext> batchMessage)
        {
            this.WorkerId = workerId;
            this.GroupId = groupId;
            this.Consumer = consumer;
            this.Message = batchMessage;
        }

        public int WorkerId { get; }

        public byte[] PartitionKey => null;

        public object Message { get; private set; }

        public IMessageHeaders Headers { get; } = new MessageHeaders();

        public string Topic => null;

        public int? Partition => null;

        public long? Offset => null;

        public string GroupId { get; }

        public IMessageContextConsumer Consumer { get; }

        public void TransformMessage(object message) => this.Message = message;

        public IMessageContext Clone() => (IMessageContext) this.MemberwiseClone();
    }
}
