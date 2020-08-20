namespace KafkaFlow.Client.Protocol.Messages
{
    /// <summary>
    /// Used to create ListOffsets responses
    /// </summary>
    public interface IListOffsetsResponse : IResponse
    {
        /// <summary>
        /// Used to create a topic entity
        /// </summary>
        public interface ITopic : IResponse
        {
            /// <summary>
            /// Gets the topic name
            /// </summary>
            string Name { get; }

            /// <summary>
            /// Gets the list of partitions
            /// </summary>
            IPartition[] Partitions { get; }
        }

        /// <summary>
        ///  Used to create a partition entity
        /// </summary>
        public interface IPartition : IResponse
        {
            /// <summary>
            /// Gets the partition index
            /// </summary>
            int Id { get; }

            /// <summary>
            /// Gets the partition error code, or 0 if there was no error
            /// </summary>
            ErrorCode ErrorCode { get; }

            /// <summary>
            /// Gets the timestamp associated with the returned offset
            /// </summary>
            long Timestamp { get; }

            /// <summary>
            /// Gets the offset
            /// </summary>
            long Offset { get; }

            /// <summary>
            /// Gets the leader epoch of the partition
            /// </summary>
            int LeaderEpoch { get; }
        }

        /// <summary>
        /// Gets the duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request did not violate any quota
        /// </summary>
        int ThrottleTimeMs { get; }

        /// <summary>
        /// Gets the topics list
        /// </summary>
        ITopic[] Topics { get; }
    }
}
