namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;

    internal class KafkaConfiguration
    {
        private readonly List<ClusterConfiguration> clusters = new();

        public Action<IDependencyResolver> OnStopHandler = _ => { };

        public IReadOnlyCollection<ClusterConfiguration> Clusters => this.clusters;

        public void AddClusters(IEnumerable<ClusterConfiguration> configurations) => this.clusters.AddRange(configurations);
    }
}
