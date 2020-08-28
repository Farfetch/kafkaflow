namespace KafkaFlow.Client.Protocol.Messages.Implementations
{
    using System;
    using System.IO;

    public class SyncGroupV5Request : IRequestMessageV2<SyncGroupV5Response>
    {
        public SyncGroupV5Request(
            string groupId,
            int generationId,
            string memberId,
            Assigment[] assignments)
        {
            this.GroupId = groupId;
            this.GenerationId = generationId;
            this.MemberId = memberId;
            this.Assignments = assignments;
        }

        public ApiKey ApiKey => ApiKey.SyncGroup;
        public short ApiVersion => 5;

        public string GroupId { get; }

        public int GenerationId { get; }

        public string MemberId { get; }

        public string? GroupInstanceId { get; set; }

        public string? ProtocolType { get; set; }

        public string? ProtocolName { get; set; }

        public Assigment[] Assignments { get; }

        public TaggedField[] TaggedFields => Array.Empty<TaggedField>();

        public void Write(Stream destination)
        {
            destination.WriteCompactString(this.GroupId);
            destination.WriteInt32(this.GenerationId);
            destination.WriteCompactString(this.MemberId);
            destination.WriteCompactNullableString(this.GroupInstanceId);
            destination.WriteCompactNullableString(this.ProtocolType);
            destination.WriteCompactNullableString(this.ProtocolName);
            destination.WriteCompactArray(this.Assignments);
            destination.WriteTaggedFields(this.TaggedFields);
        }

        public class Assigment : IRequestV2
        {
            public Assigment(string memberId, byte[] metadata)
            {
                this.MemberId = memberId;
                this.Metadata = metadata;
            }

            public string MemberId { get; }

            public byte[] Metadata { get; }

            public TaggedField[] TaggedFields => Array.Empty<TaggedField>();

            public void Write(Stream destination)
            {
                destination.WriteCompactString(this.MemberId);
                destination.WriteCompactByteArray(this.Metadata);
                destination.WriteTaggedFields(this.TaggedFields);
            }
        }

        public Type ResponseType { get; }
    }
}
