namespace KafkaFlow.Client.Protocol.Messages.Implementations
{
    using System;
    using System.IO;

    public class FindCoordinatorV3Request : IRequestMessageV2<FindCoordinatorV3Response>
    {
        public FindCoordinatorV3Request(string key, byte keyType)
        {
            this.Key = key;
            this.KeyType = keyType;
        }

        public ApiKey ApiKey => ApiKey.FindCoordinator;
        public short ApiVersion => 3;

        public string Key { get; }

        public byte KeyType { get; }

        public TaggedField[] TaggedFields => Array.Empty<TaggedField>();

        public void Write(Stream destination)
        {
            destination.WriteCompactString(this.Key);
            destination.WriteByte(this.KeyType);
            destination.WriteTaggedFields(this.TaggedFields);
        }

        public Type ResponseType { get; }
    }
}
