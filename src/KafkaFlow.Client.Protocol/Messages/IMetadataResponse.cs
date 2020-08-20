namespace KafkaFlow.Client.Protocol.Messages
{
    /// <summary>
    ///  Used to create Metadata responses
    /// </summary>
    public interface IMetadataResponse : IResponse
    {
        /// <summary>
        /// Used to create a partition entity
        /// </summary>
        public interface IPartition : IResponse
        {
            /// <summary>
            /// Gets the partition error code, or 0 if there was no error
            /// </summary>
            ErrorCode Error { get; }

            /// <summary>
            /// Gets the partition identifier
            /// </summary>
            int Id { get; }

            /// <summary>
            /// Gets the partition leader identifier
            /// </summary>
            int LeaderId { get; }

            /// <summary>
            /// Gets the leader epoch of the partition.
            /// </summary>
            int LeaderEpoch { get; }

            /// <summary>
            /// Gets the set of all nodes that host this partition
            /// </summary>
            int[] ReplicaNodes { get; }

            /// <summary>
            /// Gets the set of nodes that are in sync with the leader for this partition
            /// </summary>
            int[] IsrNodes { get; }

            /// <summary>
            /// Gets the set of offline replicas of this partition.
            /// </summary>
            int[] OfflineReplicas { get; }
        }

        /// <summary>
        /// Used to create a broker entity
        /// </summary>
        public interface IBroker : IResponse
        {
            /// <summary>
            /// Gets the broker id
            /// </summary>
            int NodeId { get; }

            /// <summary>
            /// Gets the broker hostname
            /// </summary>
            string Host { get; }

            /// <summary>
            /// Gets the broker port
            /// </summary>
            int Port { get; }

            /// <summary>
            /// Gets the rack of the broker, or null if it has not been assigned to a rack
            /// </summary>
            string? Rack { get; }
        }

        /// <summary>
        /// Used to create a topic entity
        /// </summary>
        public interface ITopic : IResponse
        {
            /// <summary>
            /// Gets the topic error code, or 0 if there was no error
            /// </summary>
            ErrorCode Error { get; }

            /// <summary>
            /// Gets the topic name
            /// </summary>
            string Name { get; }

            /// <summary>
            /// Gets a value indicating whether the topic is internal or not
            /// </summary>
            bool IsInternal { get; }

            /// <summary>
            /// Gets the partitions list
            /// </summary>
            IPartition[] Partitions { get; }

            /// <summary>
            /// Gets the 32-bit bitfield to represent authorized operations for this topic.
            /// </summary>
            int TopicAuthorizedOperations { get; }
        }

        /// <summary>
        /// Gets the duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request did not violate any quota
        /// </summary>
        int ThrottleTime { get; }

        /// <summary>
        /// Gets the brokers list
        /// </summary>
        IBroker[] Brokers { get; }

        /// <summary>
        /// Gets the cluster ID that responding broker belongs to
        /// </summary>
        string? ClusterId { get; }

        /// <summary>
        ///  Gets the ID of the controller broker.
        /// </summary>
        int ControllerId { get; }

        /// <summary>
        /// Gets the topics list
        /// </summary>
        ITopic[] Topics { get; }

        /// <summary>
        /// Gets the 32-bit bitfield to represent authorized operations for this cluster.
        /// </summary>
        int ClusterAuthorizedOperations { get; }
    }
}
