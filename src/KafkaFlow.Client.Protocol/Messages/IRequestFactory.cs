namespace KafkaFlow.Client.Protocol.Messages
{
    public interface IRequestFactory
    {
        IMetadataRequest CreateMetadata();

        IOffsetFetchRequest CreateOffsetFetch(string groupId, string topicName, int[] partitions);

        IListOffsetsRequest CreateListOffset(string topicName, int[] partitions);
    }
}
