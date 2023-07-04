namespace KafkaFlow.Clusters
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using KafkaFlow.Configuration;

    /// <summary>
    /// Provides access to Cluster administration
    /// </summary>
    public interface IClusterManager
    {
        /// <summary>
        /// Gets the unique cluster name defined in the configuration
        /// </summary>
        string ClusterName { get; }

        /// <summary>
        /// Create Topics when building Cluster Configuration if the topic doesn't exist.
        /// </summary>
        /// <param name="configuration">Topics to create</param>
        /// <returns></returns>
        public Task CreateIfNotExistsAsync(IEnumerable<TopicConfiguration> configuration);

        /// <summary>
        /// Retrieves the metadata for a specified topic.
        /// This includes information about the topic as well as its partitions.
        /// </summary>
        /// <param name="topicName">The name of the topic for which to retrieve metadata.</param>
        /// <returns>A TopicMetadata object that contains information about the topic and its partitions.</returns>
        ValueTask<TopicMetadata> GetTopicMetadataAsync(string topicName);

        /// <summary>
        /// Retrieves the offsets for a specified consumer group and a collection of topics.
        /// The offsets indicate where the consumer group is in its consumption of the topics.
        /// </summary>
        /// <param name="consumerGroup">The name of the consumer group for which to retrieve offsets.</param>
        /// <param name="topicsName">A collection of topic names for which to retrieve offsets.</param>
        /// <returns>An enumerable collection of TopicPartitionOffset objects,
        /// each of which contains offset information for a specific topic partition in the consumer group.</returns>
        Task<IEnumerable<TopicPartitionOffset>> GetConsumerGroupOffsetsAsync(
            string consumerGroup,
            IEnumerable<string> topicsName);
    }
}
