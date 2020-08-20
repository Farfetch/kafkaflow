namespace KafkaFlow.Client.Protocol.Messages
{
    /// <summary>
    /// Used to create OffsetFetch responses
    /// </summary>
    public interface IOffsetFetchResponse : IResponse
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
            /// Gets the partitions list
            /// </summary>
            IPartition[] Partitions { get; }
        }

        /// <summary>
        /// Used to create a partition entity
        /// </summary>
        public interface IPartition : IResponse
        {
            /// <summary>
            /// Gets the partition identifier
            /// </summary>
            int Id { get; }

            /// <summary>
            /// Gets the committed message offset
            /// </summary>
            long CommittedOffset { get; }

            /// <summary>
            /// Gets the leader epoch
            /// </summary>
            int CommittedLeaderEpoch { get; }

            /// <summary>
            /// Gets the partition metadata
            /// </summary>
            string Metadata { get; }

            /// <summary>
            /// Gets the partition error code, or 0 if there was no error
            /// </summary>
            ErrorCode ErrorCode { get; }
        }

        /// <summary>
        /// Gets the duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request did not violate any quota
        /// </summary>
        int ThrottleTimeMs { get; }

        /// <summary>
        /// Gets the topics list
        /// </summary>
        ITopic[] Topics { get; }

        /// <summary>
        /// The top-level error code, or 0 if there was no error
        /// </summary>
        ErrorCode Error { get; }
    }
}
