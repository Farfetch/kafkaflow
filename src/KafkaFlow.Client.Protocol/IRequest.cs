namespace KafkaFlow.Client.Protocol
{
    using KafkaFlow.Client.Protocol.Streams;

    public interface IRequest
    {
        public void Write(MemoryWritter destination);
    }
}
