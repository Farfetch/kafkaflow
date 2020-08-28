namespace KafkaFlow.Client.Protocol.Messages.Implementations
{
    using System.IO;

    public class FindCoordinatorV3Response : IResponseV2
    {
        public int ThrottleTimeMs { get; private set; }

        public ErrorCode Error { get; private set; }

        public string ErrorMessage { get; private set; }

        public int NodeId { get; private set; }

        public string Host { get; private set; }

        public int Port { get; private set; }

        public TaggedField[] TaggedFields { get; private set; }

        public void Read(Stream source)
        {
            this.ThrottleTimeMs = source.ReadInt32();
            this.Error = source.ReadErrorCode();
            this.ErrorMessage = source.ReadCompactString();
            this.NodeId = source.ReadInt32();
            this.Host = source.ReadCompactString();
            this.Port = source.ReadInt32();
            this.TaggedFields = source.ReadTaggedFields();
        }
    }
}