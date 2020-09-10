namespace KafkaFlow.Client.Protocol.Messages.Implementations
{
    public interface IJoinGroupResponse : IResponse
    {
        int ThrottleTimeMs { get; }

        ErrorCode Error { get; }

        int GenerationId { get; }

        string? ProtocolType { get; }

        string? ProtocolName { get; }

        string LeaderId { get; }

        string MemberId { get; }

        IMember[] Members { get; }

        TaggedField[] TaggedFields { get; }

        public interface IMember : IResponse
        {
            string MemberId { get; }

            string? GroupInstanceId { get; }

            byte[] Metadata { get; }

            TaggedField[] TaggedFields { get; }
        }
    }
}