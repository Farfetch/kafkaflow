namespace KafkaFlow.Configuration
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the kafka configuration values
    /// </summary>
    public class KafkaConfiguration
    {
        private readonly List<ClusterConfiguration> clusters = new();

        /// <summary>
        /// Gets the cluster configuration list
        /// </summary>
        public IReadOnlyCollection<ClusterConfiguration> Clusters => this.clusters;

        /// <summary>
        /// Adds a list of cluster configurations
        /// </summary>
        /// <param name="configurations">A list of cluster configurations</param>
        public void AddClusters(IEnumerable<ClusterConfiguration> configurations) =>
            this.clusters.AddRange(configurations);
    }
}
