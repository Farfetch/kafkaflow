namespace KafkaFlow.Client.Protocol.Messages
{
    /// <summary>
    /// Used to create ListOffsets requests
    /// </summary>
    public interface IListOffsetsRequest : IRequestMessage<IListOffsetsResponse>
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
            /// Gets the list of partitions
            /// </summary>
            IPartition[] Partitions { get; }
        }

        /// <summary>
        ///  Used to create a partition entity
        /// </summary>
        public interface IPartition : IRequest
        {
            /// <summary>
            /// Gets the partition index
            /// </summary>
            int PartitionIndex { get; }

            /// <summary>
            /// Gets the current leader epoch of the partition
            /// </summary>
            int CurrentLeaderEpoch { get; }

            /// <summary>
            /// Gets the current timestamp
            /// </summary>
            long Timestamp { get; }
        }

        /// <summary>
        /// Gets the broker ID of the requestor, or -1 if this request is being made by a normal consumer.
        /// </summary>
        int ReplicaId { get; }

        /// <summary>
        /// Gets the visibility of transactional records
        /// </summary>
        byte IsolationLevel { get; }

        /// <summary>
        /// Gets the partitions list
        /// </summary>
        ITopic[] Topics { get; }
    }
}
