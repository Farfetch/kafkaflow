namespace KafkaFlow.Client.Protocol
{
    using System.IO;
    using KafkaFlow.Client.Protocol.Messages;
    using KafkaFlow.Client.Protocol.Messages.Implementations;

    public interface IRequest
    {
        public void Write(Stream destination);
    }

    public interface IRequestV2 : IRequest
    {
        public TaggedField[] TaggedFields { get; }
    }
}
