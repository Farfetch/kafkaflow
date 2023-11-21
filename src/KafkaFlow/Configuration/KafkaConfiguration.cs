using System.Collections.Generic;

namespace KafkaFlow.Configuration
{
    /// <summary>
    /// Represents the kafka configuration values
    /// </summary>
    public class KafkaConfiguration
    {
        private readonly List<ClusterConfiguration> _clusters = new();

        /// <summary>
        /// Gets the cluster configuration list
        /// </summary>
        public IReadOnlyCollection<ClusterConfiguration> Clusters => _clusters;

        /// <summary>
        /// Adds a list of cluster configurations
        /// </summary>
        /// <param name="configurations">A list of cluster configurations</param>
        public void AddClusters(IEnumerable<ClusterConfiguration> configurations) =>
            _clusters.AddRange(configurations);
    }
}
