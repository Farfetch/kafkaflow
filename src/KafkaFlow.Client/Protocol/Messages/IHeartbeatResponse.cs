namespace KafkaFlow.Client.Protocol.Messages.Implementations
{
    public interface IHeartbeatResponse
    {
        int ThrottleTimeMs { get; }

        ErrorCode Error { get; }

        TaggedField[] TaggedFields { get; }
    }
}
