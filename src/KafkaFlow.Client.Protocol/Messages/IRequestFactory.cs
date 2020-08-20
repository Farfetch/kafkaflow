namespace KafkaFlow.Client.Protocol.Messages
{
    /// <summary>
    /// Used to create a request factory class
    /// </summary>
    public interface IRequestFactory
    {
        /// <summary>
        /// Creates a Metadata request message
        /// </summary>
        /// <param name="topics">The list of topics</param>
        /// <returns>The metadata request message</returns>
        IMetadataRequest CreateMetadata(params string[] topics);

        /// <summary>
        /// Creates an OffsetFetch request message
        /// </summary>
        /// <param name="groupId">The group identifier</param>
        /// <param name="topicName">The topic name</param>
        /// <param name="partitions">The list of partitions</param>
        /// <returns></returns>
        IOffsetFetchRequest CreateOffsetFetch(string groupId, string topicName, int[] partitions);

        /// <summary>
        /// Creates a ListOffset request message
        /// </summary>
        /// <param name="topicName">The topic name</param>
        /// <param name="partitions">The list of partitions</param>
        /// <returns></returns>
        IListOffsetsRequest CreateListOffset(string topicName, int[] partitions);
    }
}
