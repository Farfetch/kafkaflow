namespace KafkaFlow.Client.Protocol.Messages
{
    using System.Collections.Generic;

    public interface ILeaveGroupRequest : IRequestMessage<ILeaveGroupResponse>
    {
        string GroupId { get; }

        List<IMember> Members { get; }

        TaggedField[] TaggedFields { get; }

        IMember CreateMember();

        public interface IMember : IRequest
        {
            string MemberId { get; }

            string? GroupInstanceId { get; }

            TaggedField[] TaggedFields { get; }
        }
    }
}
