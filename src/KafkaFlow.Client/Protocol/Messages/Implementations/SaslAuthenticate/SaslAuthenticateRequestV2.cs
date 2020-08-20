namespace KafkaFlow.Client.Protocol.Messages.Implementations.SaslAuthenticate
{
    using System;
    using KafkaFlow.Client.Protocol.Streams;

    internal class SaslAuthenticateRequestV2 : ISaslAuthenticateRequest, ITaggedFields
    {
        public SaslAuthenticateRequestV2(byte[] authBytes)
        {
            this.AuthBytes = authBytes;
        }

        public ApiKey ApiKey => ApiKey.SaslAuthenticate;

        public short ApiVersion => 2;

        public Type ResponseType => typeof(SaslAuthenticateResponseV2);

        public byte[] AuthBytes { get; }

        public TaggedField[] TaggedFields { get; } = Array.Empty<TaggedField>();

        void IRequest.Write(MemoryWriter destination)
        {
            destination.WriteCompactByteArray(this.AuthBytes);
            destination.WriteTaggedFields(this.TaggedFields);
        }
    }
}
