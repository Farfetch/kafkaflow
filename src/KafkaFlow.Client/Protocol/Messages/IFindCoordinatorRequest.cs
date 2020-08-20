namespace KafkaFlow.Client.Protocol.Messages
{
    public interface IFindCoordinatorRequest
    {
        string Key { get; }

        byte KeyType { get; }

        TaggedField[] TaggedFields { get; }
    }
}
