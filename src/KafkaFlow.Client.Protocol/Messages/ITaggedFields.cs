namespace KafkaFlow.Client.Protocol.Messages
{
    using KafkaFlow.Client.Protocol.Messages.Implementations;

    public interface ITaggedFields
    {
        public TaggedField[] TaggedFields { get; }
    }
}
