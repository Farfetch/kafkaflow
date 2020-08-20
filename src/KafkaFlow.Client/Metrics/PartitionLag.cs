namespace KafkaFlow.Client.Metrics
{
    /// <summary>
    /// Struct containing the lag of a specific partition
    /// </summary>
    public readonly struct PartitionLag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartitionLag"/> struct.
        /// </summary>
        /// <param name="partition">The partition index</param>
        /// <param name="lag">The Lag Value</param>
        internal PartitionLag(int partition, long lag)
        {
            this.Partition = partition;
            this.Lag = lag;
        }

        /// <summary>
        /// Gets the partition index
        /// </summary>
        public int Partition { get; }

        /// <summary>
        /// Gets the lag value
        /// </summary>
        public long Lag { get; }
    }
}
