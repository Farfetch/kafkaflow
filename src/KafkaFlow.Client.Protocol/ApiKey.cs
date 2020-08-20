namespace KafkaFlow.Client.Protocol
{
    public enum ApiKey : short
    {
        Produce = 0,
        Fetch = 1,
        Offsets = 2,
        Metadata = 3,
        LeaderAndIsr = 4,
        StopReplica = 5,
        UpdateMetadata = 6,
        ControlledShutdown = 7,
        OffsetCommit = 8,
        OffsetFetch = 9,
        FindCoordinator = 10,
        JoinGroup = 11,
        Heartbeat = 12,
        LeaveGroup = 13,
        SyncGroup = 14,
        DescribeGroups = 15,
        ListGroups = 16,
        SaslHandshake = 17,
        ApiVersions = 18,
    }
}
