namespace KafkaFlow.Client.Protocol.Messages
{
    using System;
    using System.IO;

    public class HeartbeatV4Request : IRequestMessageV2<HeartbeatV4Response>
    {
        public HeartbeatV4Request(string groupId, int generationId, string memberId)
        {
            this.GroupId = groupId;
            this.GenerationId = generationId;
            this.MemberId = memberId;
        }

        public ApiKey ApiKey => ApiKey.Heartbeat;

        public short ApiVersion => 4;

        public string GroupId { get; }

        public int GenerationId { get; }

        public string MemberId { get; }

        public string? GroupInstanceId { get; set; }

        public TaggedField[] TaggedFields => Array.Empty<TaggedField>();

        public void Write(Stream destination)
        {
            destination.WriteCompactString(this.GroupId);
            destination.WriteInt32(this.GenerationId);
            destination.WriteCompactString(this.MemberId);
            destination.WriteCompactNullableString(this.GroupInstanceId);
            destination.WriteTaggedFields(this.TaggedFields);
        }
    }
}
