namespace KafkaFlow
{
    public interface IMessageContext
    {
        int WorkerId { get; }

        byte[] PartitionKey { get; }

        object Message { get; }

        IMessageHeaders Headers { get; }

        string Topic { get; }

        int? Partition { get; }

        long? Offset { get; }

        string GroupId { get; }

        IMessageContextConsumer Consumer { get; }

        void TransformMessage(object message);

        IMessageContext Clone();
    }
}
