namespace KafkaFlow.Client.Protocol.Messages
{
    public interface IHeartbeatResponse
    {
        int ThrottleTimeMs { get; }

        ErrorCode Error { get; }

        TaggedField[] TaggedFields { get; }
    }
}
