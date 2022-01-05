namespace KafkaFlow.Client.Protocol.Messages.Implementations.SaslAuthenticate
{
    using KafkaFlow.Client.Protocol.Streams;

    public class SaslAuthenticateResponseV2 : ISaslAuthenticateResponse, ITaggedFields
    {
        public short ErrorCode { get; private set; }

        public string? ErrorMessage { get; private set; }

        public byte[] AuthBytes { get; private set; }

        public long SessionLifetimeMs { get; private set; }

        public TaggedField[] TaggedFields { get; private set; }

        public void Read(MemoryReader source)
        {
            this.ErrorCode = source.ReadInt16();
            this.ErrorMessage = source.ReadCompactNullableString();
            this.AuthBytes = source.ReadCompactByteArray();
            this.SessionLifetimeMs = source.ReadInt64();
            this.TaggedFields = source.ReadTaggedFields();
        }
    }
}
