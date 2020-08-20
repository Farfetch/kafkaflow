namespace KafkaFlow.Client.Protocol
{
    using System.IO;
    using KafkaFlow.Client.Protocol.Messages;

    public interface IResponse
    {
        void Read(Stream source);
    }

    public interface IResponseV2 : IResponse
    {
        TaggedField[] TaggedFields { get; }
    }
}
