namespace KafkaFlow.Client.Protocol
{
    using System.IO;
    using KafkaFlow.Client.Protocol.Messages;

    public interface IRequest
    {
        void Write(Stream destination);
    }

    public interface IRequestV2 : IRequest
    {
        TaggedField[] TaggedFields { get; }
    }
}
