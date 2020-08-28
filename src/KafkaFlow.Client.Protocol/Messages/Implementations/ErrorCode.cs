namespace KafkaFlow.Client.Protocol.Messages.Implementations
{
    using System.ComponentModel;

    public enum ErrorCode : short
    {
        [Description("The server experienced an unexpected error when processing the request.")]
        UnknownServerError = -1,

        [Description("")]
        None = 0,

        [Description("The requested offset is not within the range of offsets maintained by the server.")]
        OffsetOutOfRange = 1,

        [Description(
            "This message has failed its CRC checksum, exceeds the valid size, has a null key for a compacted topic, or is otherwise corrupt.")]
        CorruptMessage = 2,

        [Description("This server does not host this topic-partition.")]
        UnknownTopicOrPartition = 3,

        [Description("The requested fetch size is invalid.")]
        InvalidFetchSize = 4,

        [Description("There is no leader for this topic-partition as we are in the middle of a leadership election.")]
        LeaderNotAvailable = 5,

        [Description(
            "For requests intended only for the leader, this error indicates that the broker is not the current leader. For requests intended for any replica, this error indicates that the broker is not a replica of the topic partition.")]
        NotLeaderOrFollower = 6,

        [Description("The request timed out.")]
        RequestTimedOut = 7,

        [Description("The broker is not available.")]
        BrokerNotAvailable = 8,

        [Description(
            "The replica is not available for the requested topic-partition. Produce/Fetch requests and other requests intended only for the leader or follower return NOT_LEADER_OR_FOLLOWER if the broker is not a replica of the topic-partition.")]
        ReplicaNotAvailable = 9,

        [Description("The request included a message larger than the max message size the server will accept.")]
        MessageTooLarge = 10,

        [Description("The controller moved to another broker.")]
        StaleControllerEpoch = 11,

        [Description("The metadata field of the offset request was too large.")]
        OffsetMetadataTooLarge = 12,

        [Description("The server disconnected before a response was received.")]
        NetworkException = 13,

        [Description("The coordinator is loading and hence can't process requests.")]
        CoordinatorLoadInProgress = 14,

        [Description("The coordinator is not available.")]
        CoordinatorNotAvailable = 15,

        [Description("This is not the correct coordinator.")]
        NotCoordinator = 16,

        [Description("The request attempted to perform an operation on an invalid topic.")]
        InvalidTopicException = 17,

        [Description("The request included message batch larger than the configured segment size on the server.")]
        RecordListTooLarge = 18,

        [Description("Messages are rejected since there are fewer in-sync replicas than required.")]
        NotEnoughReplicas = 19,

        [Description("Messages are written to the log, but to fewer in-sync replicas than required.")]
        NotEnoughReplicasAfterAppend = 20,

        [Description("Produce request specified an invalid value for required acks.")]
        InvalidRequiredAcks = 21,

        [Description("Specified group generation id is not valid.")]
        IllegalGeneration = 22,

        [Description(
            "The group member's supported protocols are incompatible with those of existing members or first group member tried to join with empty protocol type or empty protocol list.")]
        InconsistentGroupProtocol = 23,

        [Description("The configured groupId is invalid.")]
        InvalidGroupId = 24,

        [Description("The coordinator is not aware of this member.")]
        UnknownMemberId = 25,

        [Description(
            "The session timeout is not within the range allowed by the broker (as configured by group.min.session.timeout.ms and group.max.session.timeout.ms).")]
        InvalidSessionTimeout = 26,

        [Description("The group is rebalancing, so a rejoin is needed.")]
        RebalanceInProgress = 27,

        [Description("The committing offset data size is not valid.")]
        InvalidCommitOffsetSize = 28,

        [Description("Topic authorization failed.")]
        TopicAuthorizationFailed = 29,

        [Description("Group authorization failed.")]
        GroupAuthorizationFailed = 30,

        [Description("Cluster authorization failed.")]
        ClusterAuthorizationFailed = 31,

        [Description("The timestamp of the message is out of acceptable range.")]
        InvalidTimestamp = 32,

        [Description("The broker does not support the requested SASL mechanism.")]
        UnsupportedSaslMechanism = 33,

        [Description("Request is not valid given the current SASL state.")]
        IllegalSaslState = 34,

        [Description("The version of API is not supported.")]
        UnsupportedVersion = 35,

        [Description("Topic with this name already exists.")]
        TopicAlreadyExists = 36,

        [Description("Number of partitions is below 1.")]
        InvalidPartitions = 37,

        [Description("Replication factor is below 1 or larger than the number of available brokers.")]
        InvalidReplicationFactor = 38,

        [Description("Replica assignment is invalid.")]
        InvalidReplicaAssignment = 39,

        [Description("Configuration is invalid.")]
        InvalidConfig = 40,

        [Description("This is not the correct controller for this cluster.")]
        NotController = 41,

        [Description(
            "This most likely occurs because of a request being malformed by the client library or the message was sent to an incompatible broker. See the broker logs for more details.")]
        InvalidRequest = 42,

        [Description("The message format version on the broker does not support the request.")]
        UnsupportedForMessageFormat = 43,

        [Description("Request parameters do not satisfy the configured policy.")]
        PolicyViolation = 44,

        [Description("The broker received an out of order sequence number.")]
        OutOfOrderSequenceNumber = 45,

        [Description("The broker received a duplicate sequence number.")]
        DuplicateSequenceNumber = 46,

        [Description(
            "Producer attempted an operation with an old epoch. Either there is a newer producer with the same transactionalId, or the producer's transaction has been expired by the broker.")]
        InvalidProducerEpoch = 47,

        [Description("The producer attempted a transactional operation in an invalid state.")]
        InvalidTxnState = 48,

        [Description("The producer attempted to use a producer id which is not currently assigned to its transactional id.")]
        InvalidProducerIdMapping = 49,

        [Description(
            "The transaction timeout is larger than the maximum value allowed by the broker (as configured by transaction.max.timeout.ms).")]
        InvalidTransactionTimeout = 50,

        [Description(
            "The producer attempted to update a transaction while another concurrent operation on the same transaction was ongoing.")]
        ConcurrentTransactions = 51,

        [Description(
            "Indicates that the transaction coordinator sending a WriteTxnMarker is no longer the current coordinator for a given producer.")]
        TransactionCoordinatorFenced = 52,

        [Description("Transactional Id authorization failed.")]
        TransactionalIdAuthorizationFailed = 53,

        [Description("Security features are disabled.")]
        SecurityDisabled = 54,

        [Description(
            "The broker did not attempt to execute this operation. This may happen for batched RPCs where some operations in the batch failed, causing the broker to respond without trying the rest.")]
        OperationNotAttempted = 55,

        [Description("Disk error when trying to access log file on the disk.")]
        KafkaStorageError = 56,

        [Description("The user-specified log directory is not found in the broker config.")]
        LogDirNotFound = 57,

        [Description("SASL Authentication failed.")]
        SaslAuthenticationFailed = 58,

        [Description(
            "This exception is raised by the broker if it could not locate the producer metadata associated with the producerId in question. This could happen if, for instance, the producer's records were deleted because their retention time had elapsed. Once the last records of the producerId are removed, the producer's metadata is removed from the broker, and future appends by the producer will return this exception.")]
        UnknownProducerId = 59,

        [Description("A partition reassignment is in progress.")]
        ReassignmentInProgress = 60,

        [Description("Delegation Token feature is not enabled.")]
        DelegationTokenAuthDisabled = 61,

        [Description("Delegation Token is not found on server.")]
        DelegationTokenNotFound = 62,

        [Description("Specified Principal is not valid Owner/Renewer.")]
        DelegationTokenOwnerMismatch = 63,

        [Description(
            "Delegation Token requests are not allowed on PLAINTEXT/1-way SSL channels and on delegation token authenticated channels.")]
        DelegationTokenRequestNotAllowed = 64,

        [Description("Delegation Token authorization failed.")]
        DelegationTokenAuthorizationFailed = 65,

        [Description("Delegation Token is expired.")]
        DelegationTokenExpired = 66,

        [Description("Supplied principalType is not supported.")]
        InvalidPrincipalType = 67,

        [Description("The group is not empty.")]
        NonEmptyGroup = 68,

        [Description("The group id does not exist.")]
        GroupIdNotFound = 69,

        [Description("The fetch session ID was not found.")]
        FetchSessionIdNotFound = 70,

        [Description("The fetch session epoch is invalid.")]
        InvalidFetchSessionEpoch = 71,

        [Description("There is no listener on the leader broker that matches the listener on which metadata request was processed.")]
        ListenerNotFound = 72,

        [Description("Topic deletion is disabled.")]
        TopicDeletionDisabled = 73,

        [Description("The leader epoch in the request is older than the epoch on the broker.")]
        FencedLeaderEpoch = 74,

        [Description("The leader epoch in the request is newer than the epoch on the broker.")]
        UnknownLeaderEpoch = 75,

        [Description("The requesting client does not support the compression type of given partition.")]
        UnsupportedCompressionType = 76,

        [Description("Broker epoch has changed.")]
        StaleBrokerEpoch = 77,

        [Description(
            "The leader high watermark has not caught up from a recent leader election so the offsets cannot be guaranteed to be monotonically increasing.")]
        OffsetNotAvailable = 78,

        [Description("The group member needs to have a valid member id before actually entering a consumer group.")]
        MemberIdRequired = 79,

        [Description("The preferred leader was not available.")]
        PreferredLeaderNotAvailable = 80,

        [Description("The consumer group has reached its max size.")]
        GroupMaxSizeReached = 81,

        [Description(
            "The broker rejected this static consumer since another consumer with the same group.instance.id has registered with a different member.id.")]
        FencedInstanceId = 82,

        [Description("Eligible topic partition leaders are not available.")]
        EligibleLeadersNotAvailable = 83,

        [Description("Leader election not needed for topic partition.")]
        ElectionNotNeeded = 84,

        [Description("No partition reassignment is in progress.")]
        NoReassignmentInProgress = 85,

        [Description("Deleting offsets of a topic is forbidden while the consumer group is actively subscribed to it.")]
        GroupSubscribedToTopic = 86,

        [Description("This record has failed the validation on broker and hence will be rejected.")]
        InvalidRecord = 87,

        [Description("There are unstable offsets that need to be cleared.")]
        UnstableOffsetCommit = 88,
    }
}
