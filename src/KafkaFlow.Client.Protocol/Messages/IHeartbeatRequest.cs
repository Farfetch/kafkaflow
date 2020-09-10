namespace KafkaFlow.Client.Protocol.Messages.Implementations
{
    public interface IHeartbeatRequest
    {
        string GroupId { get; }
        int GenerationId { get; }
        string MemberId { get; }
        string? GroupInstanceId { get; set; }
        TaggedField[] TaggedFields { get; }
    }
}
