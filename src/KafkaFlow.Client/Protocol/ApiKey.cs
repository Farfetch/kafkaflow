namespace KafkaFlow.Client.Protocol
{
    /// <summary>
    /// The numeric codes that the ApiKey property in the requests can take for each of the below request types.
    /// </summary>
    public enum ApiKey : short
    {
        /// <summary>
        /// Produce operation
        /// </summary>
        Produce = 0,

        /// <summary>
        /// Fetch operation
        /// </summary>
        Fetch = 1,

        /// <summary>
        /// ListOffsets operation
        /// </summary>
        ListOffsets = 2,

        /// <summary>
        /// Metadata operation
        /// </summary>
        Metadata = 3,

        /// <summary>
        /// LeaderAndIsr operation
        /// </summary>
        LeaderAndIsr = 4,

        /// <summary>
        /// StopReplica operation
        /// </summary>
        StopReplica = 5,

        /// <summary>
        /// UpdateMetadata operation
        /// </summary>
        UpdateMetadata = 6,

        /// <summary>
        /// ControlledShutdown operation
        /// </summary>
        ControlledShutdown = 7,

        /// <summary>
        /// OffsetCommit operation
        /// </summary>
        OffsetCommit = 8,

        /// <summary>
        /// OffsetFetch operation
        /// </summary>
        OffsetFetch = 9,

        /// <summary>
        /// FindCoordinator operation
        /// </summary>
        FindCoordinator = 10,

        /// <summary>
        /// JoinGroup operation
        /// </summary>
        JoinGroup = 11,

        /// <summary>
        /// Heartbeat operation
        /// </summary>
        Heartbeat = 12,

        /// <summary>
        /// LeaveGroup operation
        /// </summary>
        LeaveGroup = 13,

        /// <summary>
        /// SyncGroup operation
        /// </summary>
        SyncGroup = 14,

        /// <summary>
        /// DescribeGroups operation
        /// </summary>
        DescribeGroups = 15,

        /// <summary>
        /// ListGroups operation
        /// </summary>
        ListGroups = 16,

        /// <summary>
        /// SaslHandshake operation
        /// </summary>
        SaslHandshake = 17,

        /// <summary>
        /// ApiVersions operation
        /// </summary>
        ApiVersions = 18,

        /// <summary>
        /// CreateTopics operation
        /// </summary>
        CreateTopics = 19,

        /// <summary>
        /// DeleteTopics operation
        /// </summary>
        DeleteTopics = 20,

        /// <summary>
        /// DeleteRecords operation
        /// </summary>
        DeleteRecords = 21,

        /// <summary>
        /// InitProducerId operation
        /// </summary>
        InitProducerId = 22,

        /// <summary>
        /// OffsetForLeaderEpoch operation
        /// </summary>
        OffsetForLeaderEpoch = 23,

        /// <summary>
        /// AddPartitionsToTxn operation
        /// </summary>
        AddPartitionsToTxn = 24,

        /// <summary>
        /// AddOffsetsToTxn operation
        /// </summary>
        AddOffsetsToTxn = 25,

        /// <summary>
        /// EndTxn operation
        /// </summary>
        EndTxn = 26,

        /// <summary>
        /// WriteTxnMarkers operation
        /// </summary>
        WriteTxnMarkers = 27,

        /// <summary>
        /// TxnOffsetCommit operation
        /// </summary>
        TxnOffsetCommit = 28,

        /// <summary>
        /// DescribeAcls operation
        /// </summary>
        DescribeAcls = 29,

        /// <summary>
        /// CreateAcls operation
        /// </summary>
        CreateAcls = 30,

        /// <summary>
        /// DeleteAcls operation
        /// </summary>
        DeleteAcls = 31,

        /// <summary>
        /// DescribeConfigs operation
        /// </summary>
        DescribeConfigs = 32,

        /// <summary>
        /// AlterConfigs operation
        /// </summary>
        AlterConfigs = 33,

        /// <summary>
        /// AlterReplicaLogDirs operation
        /// </summary>
        AlterReplicaLogDirs = 34,

        /// <summary>
        /// DescribeLogDirs operation
        /// </summary>
        DescribeLogDirs = 35,

        /// <summary>
        /// SaslAuthenticate operation
        /// </summary>
        SaslAuthenticate = 36,

        /// <summary>
        /// CreatePartitions operation
        /// </summary>
        CreatePartitions = 37,

        /// <summary>
        /// CreateDelegationToken operation
        /// </summary>
        CreateDelegationToken = 38,

        /// <summary>
        /// RenewDelegationToken operation
        /// </summary>
        RenewDelegationToken = 39,

        /// <summary>
        /// ExpireDelegationToken operation
        /// </summary>
        ExpireDelegationToken = 40,

        /// <summary>
        /// DescribeDelegationToken operation
        /// </summary>
        DescribeDelegationToken = 41,

        /// <summary>
        /// DeleteGroups operation
        /// </summary>
        DeleteGroups = 42,

        /// <summary>
        /// ElectLeaders operation
        /// </summary>
        ElectLeaders = 43,

        /// <summary>
        /// IncrementalAlterConfigs operation
        /// </summary>
        IncrementalAlterConfigs = 44,

        /// <summary>
        /// AlterPartitionReassignments operation
        /// </summary>
        AlterPartitionReassignments = 45,

        /// <summary>
        /// ListPartitionReassignments operation
        /// </summary>
        ListPartitionReassignments = 46,

        /// <summary>
        /// OffsetDelete operation
        /// </summary>
        OffsetDelete = 47,

        /// <summary>
        /// DescribeClientQuotas operation
        /// </summary>
        DescribeClientQuotas = 48,

        /// <summary>
        /// AlterClientQuotas operation
        /// </summary>
        AlterClientQuotas = 49,

        /// <summary>
        /// DescribeUserScramCredentials operation
        /// </summary>
        DescribeUserScramCredentials = 50,

        /// <summary>
        /// AlterUserScramCredentials operation
        /// </summary>
        AlterUserScramCredentials = 51,

        /// <summary>
        /// AlterIsr operation
        /// </summary>
        AlterIsr = 56,

        /// <summary>
        /// UpdateFeatures operation
        /// </summary>
        UpdateFeatures = 57,

        /// <summary>
        /// DescribeCluster operation
        /// </summary>
        DescribeCluster = 60,

        /// <summary>
        /// DescribeProducers operation
        /// </summary>
        DescribeProducers = 61,

        /// <summary>
        /// DescribeTransactions operation
        /// </summary>
        DescribeTransactions = 65,

        /// <summary>
        /// ListTransactions operation
        /// </summary>
        ListTransactions = 66,

        /// <summary>
        /// AllocateProducerIds operation
        /// </summary>
        AllocateProducerIds = 67,
    }
}
