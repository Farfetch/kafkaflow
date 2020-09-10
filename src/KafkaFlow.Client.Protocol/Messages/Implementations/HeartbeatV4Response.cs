namespace KafkaFlow.Client.Protocol.Messages.Implementations
{
    using System.IO;
    using KafkaFlow.Client.Protocol.Streams;

    public class HeartbeatV4Response : IResponse, ITaggedFields, IHeartbeatResponse
    {
        public int ThrottleTimeMs { get; private set; }

        public ErrorCode Error { get; private set; }

        public TaggedField[] TaggedFields { get; private set; }

        public void Read(Stream source)
        {
            this.ThrottleTimeMs = source.ReadInt32();
            this.Error = source.ReadErrorCode();
            this.TaggedFields = source.ReadTaggedFields();
        }
    }
}
