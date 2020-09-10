namespace KafkaFlow.Client.Protocol
{
    using System.IO;

    public interface IRequest
    {
        public void Write(Stream destination);
    }
}
