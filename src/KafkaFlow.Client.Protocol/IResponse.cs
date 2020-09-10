namespace KafkaFlow.Client.Protocol
{
    using System.IO;

    public interface IResponse
    {
        public void Read(Stream source);
    }
}
