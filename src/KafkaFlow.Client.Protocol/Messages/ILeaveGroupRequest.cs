namespace KafkaFlow.Client.Protocol.Messages.Implementations
{
    using System.Collections.Generic;

    public interface ILeaveGroupRequest : IRequestMessage<ILeaveGroupResponse>
    {
        string GroupId { get; set; }

        List<IMember> Members { get; set; }

        TaggedField[] TaggedFields { get; set; }

        IMember CreateMember();

        public interface IMember : IRequest
        {
            string MemberId { get; set; }

            string? GroupInstanceId { get; set; }

            TaggedField[] TaggedFields { get; set; }
        }
    }
}
