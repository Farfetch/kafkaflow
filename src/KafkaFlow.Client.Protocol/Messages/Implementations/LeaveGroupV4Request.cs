namespace KafkaFlow.Client.Protocol.Messages.Implementations
{
    using System;
    using System.IO;

    public class LeaveGroupV4Request : IRequestMessageV2<LeaveGroupV4Response>
    {
        public LeaveGroupV4Request(string groupId, Member[] members)
        {
            this.GroupId = groupId;
            this.Members = members;
        }

        public ApiKey ApiKey => ApiKey.LeaveGroup;

        public short ApiVersion => 4;

        public string GroupId { get; }

        public Member[] Members { get; }

        public TaggedField[] TaggedFields => Array.Empty<TaggedField>();

        public void Write(Stream destination)
        {
            destination.WriteCompactString(this.GroupId);
            destination.WriteCompactArray(this.Members);
            destination.WriteTaggedFields(this.TaggedFields);
        }

        public class Member : IRequestV2
        {
            public Member(string memberId, string? groupInstanceId)
            {
                this.MemberId = memberId;
                this.GroupInstanceId = groupInstanceId;
            }

            public string MemberId { get; }

            public string? GroupInstanceId { get; }

            public TaggedField[] TaggedFields => Array.Empty<TaggedField>();

            public void Write(Stream destination)
            {
                destination.WriteCompactString(this.MemberId);
                destination.WriteCompactNullableString(this.GroupInstanceId);
                destination.WriteTaggedFields(this.TaggedFields);
            }
        }

        public Type ResponseType { get; }
    }
}
