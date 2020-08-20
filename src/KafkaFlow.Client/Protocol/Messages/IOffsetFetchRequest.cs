namespace KafkaFlow.Client.Protocol.Messages
{
    using System.Collections.Generic;

    /// <summary>
    ///  Used to create OffsetFetch requests
    /// </summary>
    public interface IOffsetFetchRequest : IRequestMessage<IOffsetFetchResponse>
    {
        /// <summary>
        /// Gets the group to fetch offsets for
        /// </summary>
        string GroupId { get; }

        /// <summary>
        /// Adds a topic and its partitions to the request
        /// </summary>
        /// <param name="name">The topic name</param>
        /// <param name="partitions">The partitions number</param>
        void AddTopic(string name, IReadOnlyList<int> partitions);
    }
}
