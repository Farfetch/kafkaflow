namespace KafkaFlow.Client.Protocol.Messages.Implementations
{
    using System;
    using System.IO;

    public class JoinGroupV7Request : IRequestMessageV2<JoinGroupV7Response>
    {
        public JoinGroupV7Request(
            string groupId,
            int sessionTimeoutMs,
            int rebalanceTimeoutMs,
            string memberId,
            string? groupInstanceId,
            string protocolType,
            Protocol[] supportedProtocols)
        {
            this.GroupId = groupId;
            this.SessionTimeoutMs = sessionTimeoutMs;
            this.RebalanceTimeoutMs = rebalanceTimeoutMs;
            this.MemberId = memberId;
            this.GroupInstanceId = groupInstanceId;
            this.ProtocolType = protocolType;
            this.SupportedProtocols = supportedProtocols;
        }

        public ApiKey ApiKey => ApiKey.JoinGroup;
        public short ApiVersion => 7;

        public string GroupId { get; }

        public int SessionTimeoutMs { get; }

        public int RebalanceTimeoutMs { get; }

        public string MemberId { get; }

        public string? GroupInstanceId { get; }

        public string ProtocolType { get; }

        public Protocol[] SupportedProtocols { get; }

        public TaggedField[] TaggedFields => Array.Empty<TaggedField>();

        public void Write(Stream destination)
        {
            destination.WriteCompactString(this.GroupId);
            destination.WriteInt32(this.SessionTimeoutMs);
            destination.WriteInt32(this.RebalanceTimeoutMs);
            destination.WriteCompactString(this.MemberId);
            destination.WriteCompactNullableString(this.GroupInstanceId);
            destination.WriteCompactString(this.ProtocolType);
            destination.WriteCompactArray(this.SupportedProtocols);
            destination.WriteTaggedFields(this.TaggedFields);
        }

        public class Protocol : IRequestV2
        {
            public Protocol(string name, byte[] metadata)
            {
                this.Name = name;
                this.Metadata = metadata;
            }

            public void Write(Stream destination)
            {
                destination.WriteCompactString(this.Name);
                destination.WriteCompactByteArray(this.Metadata);
                destination.WriteTaggedFields(this.TaggedFields);
            }

            public string Name { get; }

            public byte[] Metadata { get; }

            public TaggedField[] TaggedFields => Array.Empty<TaggedField>();
        }

        public Type ResponseType { get; }
    }
}
