namespace KafkaFlow.Client.Protocol.Messages
{
    public interface IFindCoordinatorResponse
    {
        int ThrottleTimeMs { get; }

        ErrorCode Error { get; }

        string ErrorMessage { get; }

        int NodeId { get; }

        string Host { get; }

        int Port { get; }

        TaggedField[] TaggedFields { get; }
    }
}
