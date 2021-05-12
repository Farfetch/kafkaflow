namespace KafkaFlow.Consumers
{
    /// <summary>
    /// An enum with all consumer flow status
    /// </summary>
    public enum ConsumerFlowStatus
    {
        /// <summary>
        /// When the consumer is not running
        /// </summary>
        NotRunning,

        /// <summary>
        /// When all consumer partitions are running
        /// </summary>
        Running,

        /// <summary>
        /// When the consumer has paused and running partitions at the same time
        /// </summary>
        PartiallyRunning,

        /// <summary>
        /// When all consumer partitions are paused
        /// </summary>
        Paused,
    }
}
