namespace KafkaFlow
{
    using Confluent.Kafka;

    internal class ConsumerMessageContext : IMessageContext
    {
        private readonly ConsumeResult<byte[], byte[]> result;

        public ConsumerMessageContext(
            IMessageContextConsumer consumer,
            ConsumeResult<byte[], byte[]> result,
            int workerId,
            string groupId)
        {
            this.result = result;
            this.Consumer = consumer;
            this.Message = result.Message.Value;
            this.Headers = new MessageHeaders(result.Message.Headers);
            this.WorkerId = workerId;
            this.GroupId = groupId;
        }

        public int WorkerId { get; }

        public byte[] PartitionKey => this.result.Message.Key;

        public object Message { get; private set; }

        public IMessageHeaders Headers { get; }

        public string Topic => this.result.Topic;

        public string GroupId { get; }

        public int? Partition => this.result.Partition.Value;

        public long? Offset => this.result.Offset.Value;

        public IMessageContextConsumer Consumer { get; }

        public void TransformMessage(object message)
        {
            this.Message = message;
        }

        public IMessageContext Clone() => (IMessageContext) this.MemberwiseClone();
    }
}
