namespace KafkaFlow.Configuration
{
    /// <summary>
    /// Represents a Topic configuration
    /// </summary>
    public class TopicConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TopicConfiguration"/> class.
        /// </summary>
        /// <param name="name">The topic name</param>
        /// <param name="numberOfPartitions">The number of partitions for the topic</param>
        /// <param name="replicationFactor">Replication factor for the topic</param>
        public TopicConfiguration(string name, int numberOfPartitions, short replicationFactor)
        {
            this.Name = name;
            this.NumberOfPartitions = numberOfPartitions;
            this.ReplicationFactor = replicationFactor;
        }

        /// <summary>
        /// Gets the Topic Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the number of Topic Partition
        /// </summary>
        public int NumberOfPartitions { get; }

        /// <summary>
        /// Gets the Topic Replication Factor
        /// </summary>
        public short ReplicationFactor { get; }
    }
}
