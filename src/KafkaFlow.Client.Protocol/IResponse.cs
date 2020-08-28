namespace KafkaFlow.Client.Protocol
{
    using System.IO;
    using KafkaFlow.Client.Protocol.Messages;
    using KafkaFlow.Client.Protocol.Messages.Implementations;

    public interface IResponse
    {
        public void Read(Stream source);
    }

    public interface IResponseV2 : IResponse
    {
        public TaggedField[] TaggedFields { get; }
    }
}
