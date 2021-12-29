namespace KafkaFlow.Client.Protocol
{
    using KafkaFlow.Client.Protocol.Streams;

    public interface IRequest
    {
        internal void Write(MemoryWriter destination);
    }
}
