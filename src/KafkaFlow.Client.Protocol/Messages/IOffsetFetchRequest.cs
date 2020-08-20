namespace KafkaFlow.Client.Protocol.Messages
{
    /// <summary>
    ///  Used to create OffsetFetch requests
    /// </summary>
    public interface IOffsetFetchRequest : IRequestMessage<IOffsetFetchResponse>
    {
        /// <summary>
        /// Used to create a topic entity
        /// </summary>
        public interface ITopic : IRequest
        {
            /// <summary>
            /// Gets the topic name
            /// </summary>
            string Name { get; }

            /// <summary>
            /// Gets the partitions list
            /// </summary>
            int[] Partitions { get; }
        }

        /// <summary>
        /// Gets the group to fetch offsets for
        /// </summary>
        string GroupId { get; }

        /// <summary>
        /// Gets the topics list
        /// </summary>
        ITopic[] Topics { get; }
    }
}
