namespace KafkaFlow.Client.Protocol.Messages
{
    using System.ComponentModel;

    /// <summary>
    /// The numeric codes to indicate what problem occurred on the server
    /// </summary>
    public enum ErrorCode : short
    {
        /// <summary>
        /// The server experienced an unexpected error when processing the request.
        /// </summary>
        [Description("The server experienced an unexpected error when processing the request.")]
        UnknownServerError = -1,

        /// <summary>
        /// None error occurred
        /// </summary>
        [Description("None error occurred")]
        None = 0,

        /// <summary>
        /// The requested offset is not within the range of offsets maintained by the server.
        /// </summary>
        [Description("The requested offset is not within the range of offsets maintained by the server.")]
        OffsetOutOfRange = 1,

        /// <summary>
        /// This message has failed its CRC checksum, exceeds the valid size, has a null key for a compacted topic, or is otherwise corrupt.
        /// </summary>
        [Description(
            "This message has failed its CRC checksum, exceeds the valid size, has a null key for a compacted topic, or is otherwise corrupt.")]
        CorruptMessage = 2,

        /// <summary>
        /// This server does not host this topic-partition.
        /// </summary>
        [Description("This server does not host this topic-partition.")]
        UnknownTopicOrPartition = 3,

        /// <summary>
        /// The requested fetch size is invalid.
        /// </summary>
        [Description("The requested fetch size is invalid.")]
        InvalidFetchSize = 4,

        /// <summary>
        /// There is no leader for this topic-partition as we are in the middle of a leadership election.
        /// </summary>
        [Description("There is no leader for this topic-partition as we are in the middle of a leadership election.")]
        LeaderNotAvailable = 5,

        /// <summary>
        /// "For requests intended only for the leader, this error indicates that the broker is not the current leader. For requests intended for any replica, this error indicates that the broker is not a replica of the topic partition.
        /// </summary>
        [Description(
            "For requests intended only for the leader, this error indicates that the broker is not the current leader. For requests intended for any replica, this error indicates that the broker is not a replica of the topic partition.")]
        NotLeaderOrFollower = 6,

        /// <summary>
        /// The request timed out.
        /// </summary>
        [Description("The request timed out.")]
        RequestTimedOut = 7,

        /// <summary>
        /// The broker is not available.
        /// </summary>
        [Description("The broker is not available.")]
        BrokerNotAvailable = 8,

        /// <summary>
        /// "The replica is not available for the requested topic-partition. Produce/Fetch requests and other requests intended only for the leader or follower return NOT_LEADER_OR_FOLLOWER if the broker is not a replica of the topic-partition.
        /// </summary>
        [Description(
            "The replica is not available for the requested topic-partition. Produce/Fetch requests and other requests intended only for the leader or follower return NOT_LEADER_OR_FOLLOWER if the broker is not a replica of the topic-partition.")]
        ReplicaNotAvailable = 9,

        /// <summary>
        /// The request included a message larger than the max message size the server will accept.
        /// </summary>
        [Description("The request included a message larger than the max message size the server will accept.")]
        MessageTooLarge = 10,

        /// <summary>
        /// The controller moved to another broker.
        /// </summary>
        [Description("The controller moved to another broker.")]
        StaleControllerEpoch = 11,

        /// <summary>
        /// The metadata field of the offset request was too large.
        /// </summary>
        [Description("The metadata field of the offset request was too large.")]
        OffsetMetadataTooLarge = 12,

        /// <summary>
        /// The server disconnected before a response was received.
        /// </summary>
        [Description("The server disconnected before a response was received.")]
        NetworkException = 13,

        /// <summary>
        /// The coordinator is loading and hence can't process requests.
        /// </summary>
        [Description("The coordinator is loading and hence can't process requests.")]
        CoordinatorLoadInProgress = 14,

        /// <summary>
        /// The coordinator is not available.
        /// </summary>
        [Description("The coordinator is not available.")]
        CoordinatorNotAvailable = 15,

        /// <summary>
        /// This is not the correct coordinator.
        /// </summary>
        [Description("This is not the correct coordinator.")]
        NotCoordinator = 16,

        /// <summary>
        /// The request attempted to perform an operation on an invalid topic.
        /// </summary>
        [Description("The request attempted to perform an operation on an invalid topic.")]
        InvalidTopicException = 17,

        /// <summary>
        /// The request included message batch larger than the configured segment size on the server.
        /// </summary>
        [Description("The request included message batch larger than the configured segment size on the server.")]
        RecordListTooLarge = 18,

        /// <summary>
        /// Messages are rejected since there are fewer in-sync replicas than required.
        /// </summary>
        [Description("Messages are rejected since there are fewer in-sync replicas than required.")]
        NotEnoughReplicas = 19,

        /// <summary>
        /// Messages are written to the log, but to fewer in-sync replicas than required.
        /// </summary>
        [Description("Messages are written to the log, but to fewer in-sync replicas than required.")]
        NotEnoughReplicasAfterAppend = 20,

        /// <summary>
        /// Produce request specified an invalid value for required acks.
        /// </summary>
        [Description("Produce request specified an invalid value for required acks.")]
        InvalidRequiredAcks = 21,

        /// <summary>
        /// Specified group generation id is not valid.
        /// </summary>
        [Description("Specified group generation id is not valid.")]
        IllegalGeneration = 22,

        /// <summary>
        /// "The group member's supported protocols are incompatible with those of existing members or first group member tried to join with empty protocol type or empty protocol list.
        /// </summary>
        [Description(
            "The group member's supported protocols are incompatible with those of existing members or first group member tried to join with empty protocol type or empty protocol list.")]
        InconsistentGroupProtocol = 23,

        /// <summary>
        /// The configured groupId is invalid.
        /// </summary>
        [Description("The configured groupId is invalid.")]
        InvalidGroupId = 24,

        /// <summary>
        /// The coordinator is not aware of this member.
        /// </summary>
        [Description("The coordinator is not aware of this member.")]
        UnknownMemberId = 25,

        /// <summary>
        /// "The session timeout is not within the range allowed by the broker (as configured by group.min.session.timeout.ms and group.max.session.timeout.ms).
        /// </summary>
        [Description(
            "The session timeout is not within the range allowed by the broker (as configured by group.min.session.timeout.ms and group.max.session.timeout.ms).")]
        InvalidSessionTimeout = 26,

        /// <summary>
        /// The group is rebalancing, so a rejoin is needed.
        /// </summary>
        [Description("The group is rebalancing, so a rejoin is needed.")]
        RebalanceInProgress = 27,

        /// <summary>
        /// The committing offset data size is not valid.
        /// </summary>
        [Description("The committing offset data size is not valid.")]
        InvalidCommitOffsetSize = 28,

        /// <summary>
        /// Topic authorization failed.
        /// </summary>
        [Description("Topic authorization failed.")]
        TopicAuthorizationFailed = 29,

        /// <summary>
        /// Group authorization failed.
        /// </summary>
        [Description("Group authorization failed.")]
        GroupAuthorizationFailed = 30,

        /// <summary>
        /// Cluster authorization failed.
        /// </summary>
        [Description("Cluster authorization failed.")]
        ClusterAuthorizationFailed = 31,

        /// <summary>
        /// The timestamp of the message is out of acceptable range.
        /// </summary>
        [Description("The timestamp of the message is out of acceptable range.")]
        InvalidTimestamp = 32,

        /// <summary>
        /// The broker does not support the requested SASL mechanism.
        /// </summary>
        [Description("The broker does not support the requested SASL mechanism.")]
        UnsupportedSaslMechanism = 33,

        /// <summary>
        /// Request is not valid given the current SASL state.
        /// </summary>
        [Description("Request is not valid given the current SASL state.")]
        IllegalSaslState = 34,

        /// <summary>
        /// The version of API is not supported.
        /// </summary>
        [Description("The version of API is not supported.")]
        UnsupportedVersion = 35,

        /// <summary>
        /// Topic with this name already exists.
        /// </summary>
        [Description("Topic with this name already exists.")]
        TopicAlreadyExists = 36,

        /// <summary>
        /// Number of partitions is below 1.
        /// </summary>
        [Description("Number of partitions is below 1.")]
        InvalidPartitions = 37,

        /// <summary>
        /// Replication factor is below 1 or larger than the number of available brokers.
        /// </summary>
        [Description("Replication factor is below 1 or larger than the number of available brokers.")]
        InvalidReplicationFactor = 38,

        /// <summary>
        /// Replica assignment is invalid.
        /// </summary>
        [Description("Replica assignment is invalid.")]
        InvalidReplicaAssignment = 39,

        /// <summary>
        /// Configuration is invalid.
        /// </summary>
        [Description("Configuration is invalid.")]
        InvalidConfig = 40,

        /// <summary>
        /// This is not the correct controller for this cluster.
        /// </summary>
        [Description("This is not the correct controller for this cluster.")]
        NotController = 41,

        /// <summary>
        /// "This most likely occurs because of a request being malformed by the client library or the message was sent to an incompatible broker. See the broker logs for more details.
        /// </summary>
        [Description(
            "This most likely occurs because of a request being malformed by the client library or the message was sent to an incompatible broker. See the broker logs for more details.")]
        InvalidRequest = 42,

        /// <summary>
        /// The message format version on the broker does not support the request.
        /// </summary>
        [Description("The message format version on the broker does not support the request.")]
        UnsupportedForMessageFormat = 43,

        /// <summary>
        /// Request parameters do not satisfy the configured policy.
        /// </summary>
        [Description("Request parameters do not satisfy the configured policy.")]
        PolicyViolation = 44,

        /// <summary>
        /// The broker received an out of order sequence number.
        /// </summary>
        [Description("The broker received an out of order sequence number.")]
        OutOfOrderSequenceNumber = 45,

        /// <summary>
        /// The broker received a duplicate sequence number.
        /// </summary>
        [Description("The broker received a duplicate sequence number.")]
        DuplicateSequenceNumber = 46,

        /// <summary>
        /// "Producer attempted an operation with an old epoch. Either there is a newer producer with the same transactionalId, or the producer's transaction has been expired by the broker.
        /// </summary>
        [Description(
            "Producer attempted an operation with an old epoch. Either there is a newer producer with the same transactionalId, or the producer's transaction has been expired by the broker.")]
        InvalidProducerEpoch = 47,

        /// <summary>
        /// The producer attempted a transactional operation in an invalid state.
        /// </summary>
        [Description("The producer attempted a transactional operation in an invalid state.")]
        InvalidTxnState = 48,

        /// <summary>
        /// The producer attempted to use a producer id which is not currently assigned to its transactional id.
        /// </summary>
        [Description("The producer attempted to use a producer id which is not currently assigned to its transactional id.")]
        InvalidProducerIdMapping = 49,

        /// <summary>
        /// "The transaction timeout is larger than the maximum value allowed by the broker (as configured by transaction.max.timeout.ms).
        /// </summary>
        [Description(
            "The transaction timeout is larger than the maximum value allowed by the broker (as configured by transaction.max.timeout.ms).")]
        InvalidTransactionTimeout = 50,

        /// <summary>
        /// The producer attempted to update a transaction while another concurrent operation on the same transaction was ongoing.
        /// </summary>
        [Description(
            "The producer attempted to update a transaction while another concurrent operation on the same transaction was ongoing.")]
        ConcurrentTransactions = 51,

        /// <summary>
        /// Indicates that the transaction coordinator sending a WriteTxnMarker is no longer the current coordinator for a given producer."
        /// </summary>
        [Description(
            "Indicates that the transaction coordinator sending a WriteTxnMarker is no longer the current coordinator for a given producer.")]
        TransactionCoordinatorFenced = 52,

        /// <summary>
        /// Transactional Id authorization failed.
        /// </summary>
        [Description("Transactional Id authorization failed.")]
        TransactionalIdAuthorizationFailed = 53,

        /// <summary>
        /// Security features are disabled.
        /// </summary>
        [Description("Security features are disabled.")]
        SecurityDisabled = 54,

        /// <summary>
        /// The broker did not attempt to execute this operation. This may happen for batched RPCs where some operations in the batch failed, causing the broker to respond without trying the rest.
        /// </summary>
        [Description(
            "The broker did not attempt to execute this operation. This may happen for batched RPCs where some operations in the batch failed, causing the broker to respond without trying the rest.")]
        OperationNotAttempted = 55,

        /// <summary>
        /// Disk error when trying to access log file on the disk.
        /// </summary>
        [Description("Disk error when trying to access log file on the disk.")]
        KafkaStorageError = 56,

        /// <summary>
        /// The user-specified log directory is not found in the broker config.
        /// </summary>
        [Description("The user-specified log directory is not found in the broker config.")]
        LogDirNotFound = 57,

        /// <summary>
        /// SASL Authentication failed.
        /// </summary>
        [Description("SASL Authentication failed.")]
        SaslAuthenticationFailed = 58,

        /// <summary>
        /// This exception is raised by the broker if it could not locate the producer metadata associated with the producerId in question. This could happen if, for instance, the producer's records were deleted because their retention time had elapsed. Once the last records of the producerId are removed, the producer's metadata is removed from the broker, and future appends by the producer will return this exception.
        /// </summary>
        [Description(
            "This exception is raised by the broker if it could not locate the producer metadata associated with the producerId in question. This could happen if, for instance, the producer's records were deleted because their retention time had elapsed. Once the last records of the producerId are removed, the producer's metadata is removed from the broker, and future appends by the producer will return this exception.")]
        UnknownProducerId = 59,

        /// <summary>
        /// A partition reassignment is in progress.
        /// </summary>
        [Description("A partition reassignment is in progress.")]
        ReassignmentInProgress = 60,

        /// <summary>
        /// Delegation Token feature is not enabled.
        /// </summary>
        [Description("Delegation Token feature is not enabled.")]
        DelegationTokenAuthDisabled = 61,

        /// <summary>
        /// Delegation Token is not found on server.
        /// </summary>
        [Description("Delegation Token is not found on server.")]
        DelegationTokenNotFound = 62,

        /// <summary>
        /// Specified Principal is not valid Owner/Renewer.
        /// </summary>
        [Description("Specified Principal is not valid Owner/Renewer.")]
        DelegationTokenOwnerMismatch = 63,

        /// <summary>
        /// Delegation Token requests are not allowed on PLAINTEXT/1-way SSL channels and on delegation token authenticated channels.
        /// </summary>
        [Description("Delegation Token requests are not allowed on PLAINTEXT/1-way SSL channels and on delegation token authenticated channels.")]
        DelegationTokenRequestNotAllowed = 64,

        /// <summary>
        /// Delegation Token authorization failed.
        /// </summary>
        [Description("Delegation Token authorization failed.")]
        DelegationTokenAuthorizationFailed = 65,

        /// <summary>
        /// Delegation Token is expired.
        /// </summary>
        [Description("Delegation Token is expired.")]
        DelegationTokenExpired = 66,

        /// <summary>
        /// Supplied principalType is not supported.
        /// </summary>
        [Description("Supplied principalType is not supported.")]
        InvalidPrincipalType = 67,

        /// <summary>
        /// The group is not empty.
        /// </summary>
        [Description("The group is not empty.")]
        NonEmptyGroup = 68,

        /// <summary>
        /// The group id does not exist.
        /// </summary>
        [Description("The group id does not exist.")]
        GroupIdNotFound = 69,

        /// <summary>
        /// The fetch session ID was not found.
        /// </summary>
        [Description("The fetch session ID was not found.")]
        FetchSessionIdNotFound = 70,

        /// <summary>
        /// The fetch session epoch is invalid.
        /// </summary>
        [Description("The fetch session epoch is invalid.")]
        InvalidFetchSessionEpoch = 71,

        /// <summary>
        /// There is no listener on the leader broker that matches the listener on which metadata request was processed.
        /// </summary>
        [Description("There is no listener on the leader broker that matches the listener on which metadata request was processed.")]
        ListenerNotFound = 72,

        /// <summary>
        /// Topic deletion is disabled.
        /// </summary>
        [Description("Topic deletion is disabled.")]
        TopicDeletionDisabled = 73,

        /// <summary>
        /// The leader epoch in the request is older than the epoch on the broker.
        /// </summary>
        [Description("The leader epoch in the request is older than the epoch on the broker.")]
        FencedLeaderEpoch = 74,

        /// <summary>
        /// The leader epoch in the request is newer than the epoch on the broker.
        /// </summary>
        [Description("The leader epoch in the request is newer than the epoch on the broker.")]
        UnknownLeaderEpoch = 75,

        /// <summary>
        /// The requesting client does not support the compression type of given partition.
        /// </summary>
        [Description("The requesting client does not support the compression type of given partition.")]
        UnsupportedCompressionType = 76,

        /// <summary>
        /// Broker epoch has changed.
        /// </summary>
        [Description("Broker epoch has changed.")]
        StaleBrokerEpoch = 77,

        /// <summary>
        /// The leader high watermark has not caught up from a recent leader election so the offsets cannot be guaranteed to be monotonically increasing
        /// </summary>
        [Description("The leader high watermark has not caught up from a recent leader election so the offsets cannot be guaranteed to be monotonically increasing.")]
        OffsetNotAvailable = 78,

        /// <summary>
        /// The group member needs to have a valid member id before actually entering a consumer group.
        /// </summary>
        [Description("The group member needs to have a valid member id before actually entering a consumer group.")]
        MemberIdRequired = 79,

        /// <summary>
        /// The preferred leader was not available.
        /// </summary>
        [Description("The preferred leader was not available.")]
        PreferredLeaderNotAvailable = 80,

        /// <summary>
        /// The consumer group has reached its max size.
        /// </summary>
        [Description("The consumer group has reached its max size.")]
        GroupMaxSizeReached = 81,

        /// <summary>
        /// The broker rejected this static consumer since another consumer with the same group.instance.id has registered with a different member.id.
        /// </summary>
        [Description("The broker rejected this static consumer since another consumer with the same group.instance.id has registered with a different member.id.")]
        FencedInstanceId = 82,

        /// <summary>
        /// Eligible topic partition leaders are not available.
        /// </summary>
        [Description("Eligible topic partition leaders are not available.")]
        EligibleLeadersNotAvailable = 83,

        /// <summary>
        /// Leader election not needed for topic partition.
        /// </summary>
        [Description("Leader election not needed for topic partition.")]
        ElectionNotNeeded = 84,

        /// <summary>
        /// No partition reassignment is in progress.
        /// </summary>
        [Description("No partition reassignment is in progress.")]
        NoReassignmentInProgress = 85,

        /// <summary>
        /// Deleting offsets of a topic is forbidden while the consumer group is actively subscribed to it.
        /// </summary>
        [Description("Deleting offsets of a topic is forbidden while the consumer group is actively subscribed to it.")]
        GroupSubscribedToTopic = 86,

        /// <summary>
        /// This record has failed the validation on broker and hence will be rejected.
        /// </summary>
        [Description("This record has failed the validation on broker and hence will be rejected.")]
        InvalidRecord = 87,

        /// <summary>
        /// There are unstable offsets that need to be cleared.
        /// </summary>
        [Description("There are unstable offsets that need to be cleared.")]
        UnstableOffsetCommit = 88,

        /// <summary>
        /// The throttling quota has been exceeded.
        /// </summary>
        [Description("The throttling quota has been exceeded.")]
        ThrottlingQuotaExceeded = 89,

        /// <summary>
        /// There is a newer producer with the same transactionalId which fences the current one.
        /// </summary>
        [Description("There is a newer producer with the same transactionalId which fences the current one.")]
        ProducerFenced = 90,

        /// <summary>
        /// A request illegally referred to a resource that does not exist.
        /// </summary>
        [Description("A request illegally referred to a resource that does not exist.")]
        ResourceNotFound = 91,

        /// <summary>
        /// A request illegally referred to the same resource twice.
        /// </summary>
        [Description("A request illegally referred to the same resource twice.")]
        DuplicateResource = 92,

        /// <summary>
        /// Requested credential would not meet criteria for acceptability.
        /// </summary>
        [Description("Requested credential would not meet criteria for acceptability.")]
        UnacceptableCredential = 93,

        /// <summary>
        /// Indicates that the either the sender or recipient of a voter-only request is not one of the expected voters.
        /// </summary>
        [Description("Indicates that the either the sender or recipient of a voter-only request is not one of the expected voters.")]
        InconsistentVoterSet = 94,

        /// <summary>
        /// The given update version was invalid.
        /// </summary>
        [Description("The given update version was invalid.")]
        InvalidUpdateVersion = 95,

        /// <summary>
        /// Unable to update finalized features due to an unexpected server error.
        /// </summary>
        [Description("Unable to update finalized features due to an unexpected server error.")]
        FeatureUpdateFailed = 96,

        /// <summary>
        /// Request principal deserialization failed during forwarding. This indicates an internal error on the broker cluster security setup.
        /// </summary>
        [Description("Request principal deserialization failed during forwarding. This indicates an internal error on the broker cluster security setup.")]
        PrincipalDeserializationFailure = 97,

        /// <summary>
        /// Requested snapshot was not found.
        /// </summary>
        [Description("Requested snapshot was not found.")]
        SnapshotNotFound = 98,

        /// <summary>
        /// Requested position is not greater than or equal to zero, and less than the size of the snapshot.
        /// </summary>
        [Description("Requested position is not greater than or equal to zero, and less than the size of the snapshot.")]
        PositionOutOfRange = 99,

        /// <summary>
        /// This server does not host this topic ID.
        /// </summary>
        [Description("This server does not host this topic ID.")]
        UnknownTopicId = 100,

        /// <summary>
        /// This broker ID is already in use.
        /// </summary>
        [Description("This broker ID is already in use.")]
        DuplicateBrokerRegistration = 101,

        /// <summary>
        /// The given broker ID was not registered.
        /// </summary>
        [Description("The given broker ID was not registered.")]
        BrokerIdNotRegistered = 102,

        /// <summary>
        /// The log's topic ID did not match the topic ID in the request.
        /// </summary>
        [Description("The log's topic ID did not match the topic ID in the request.")]
        InconsistentTopicId = 103,

        /// <summary>
        /// The clusterId in the request does not match that found on the server.
        /// </summary>
        [Description("The clusterId in the request does not match that found on the server.")]
        InconsistentClusterId = 104,

        /// <summary>
        /// The transactionalId could not be found.
        /// </summary>
        [Description("The transactionalId could not be found.")]
        TransactionalIdNotFound = 105,
    }
}
