namespace KafkaFlow.OpenTelemetry;

internal static class AttributeKeys
{
    public const string ClientId = "messaging.client.id";
    public const string ConsumerGroupName = "messaging.consumer.group.name";
    public const string DestinationName = "messaging.destination.name";
    public const string DestinationPartitionId = "messaging.destination.partition.id";
    public const string ErrorType = "error.type";
    public const string KafkaMessageKey = "messaging.kafka.message.key";
    public const string KafkaMessageTombstone = "messaging.kafka.message.tombstone";
    public const string KafkaOffset = "messaging.kafka.offset";
    public const string MessageBodySize = "messaging.message.body.size";
    public const string OperationName = "messaging.operation.name";
    public const string OperationType = "messaging.operation.type";
}
