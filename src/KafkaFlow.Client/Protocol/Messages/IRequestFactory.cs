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
        /// <returns>The metadata request message</returns>
        IMetadataRequest CreateMetadata();

        /// <summary>
        /// Creates an OffsetFetch request message
        /// </summary>
        /// <param name="groupId">The group identifier</param>
        /// <returns></returns>
        IOffsetFetchRequest CreateOffsetFetch(string groupId);

        /// <summary>
        /// Creates a ListOffset request message
        /// </summary>
        /// <returns></returns>
        IListOffsetsRequest CreateListOffset();

        /// <summary>
        /// Creates a Produce request
        /// </summary>
        /// <param name="acks">The message acknowledge</param>
        /// <param name="timeout">The produce timeout</param>
        /// <returns></returns>
        IProduceRequest CreateProduce(ProduceAcks acks, int timeout);
    }
}
