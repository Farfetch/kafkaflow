namespace KafkaFlow.Client.Protocol
{
    using KafkaFlow.Client.Protocol.Streams;

    public interface IResponse
    {
        public void Read(BaseMemoryStream source);
    }
}
