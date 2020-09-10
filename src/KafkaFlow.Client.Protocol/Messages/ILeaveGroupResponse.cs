namespace KafkaFlow.Client.Protocol.Messages.Implementations
{
    public interface ILeaveGroupResponse : IResponse
    {
        int ThrottleTimeMs { get; }

        ErrorCode Error { get; }

        IMember[] Members { get; }

        TaggedField[] TaggedFields { get; }

        public interface IMember : IResponse
        {
            string MemberId { get; }

            string? GroupInstanceId { get; }

            ErrorCode Error { get; }

            TaggedField[] TaggedFields { get; }
        }
    }
}
