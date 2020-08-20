namespace KafkaFlow.Client.Protocol.Messages
{
    public interface IHeartbeatRequest
    {
        string GroupId { get; }

        int GenerationId { get; }

        string MemberId { get; }

        string? GroupInstanceId { get; }

        TaggedField[] TaggedFields { get; }
    }
}
