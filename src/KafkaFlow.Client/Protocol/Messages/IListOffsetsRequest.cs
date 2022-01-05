namespace KafkaFlow.Client.Protocol.Messages
{
    using System.Collections.Generic;

    /// <summary>
    /// Used to create ListOffsets requests
    /// </summary>
    public interface IListOffsetsRequest : IRequestMessage<IListOffsetsResponse>
    {
        /// <summary>
        /// Adds a topic to the request
        /// </summary>
        /// <param name="name">Topic name</param>
        /// <param name="partitions">The partitions number</param>
        /// <returns></returns>
        public IListOffsetsRequest AddTopic(string name, IEnumerable<int> partitions);
    }
}
