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
    }
}
