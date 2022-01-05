namespace KafkaFlow.Consumers
{
    using System.Collections.Generic;
    using System.Linq;
    using KafkaFlow.Client;

    internal class ClusterAccessor : IClusterAccessor
    {
        private readonly IDictionary<string, IKafkaCluster> clusters = new Dictionary<string, IKafkaCluster>();

        public IEnumerable<IKafkaCluster> All => this.clusters.Values.AsEnumerable();

        public IKafkaCluster this[string name] => this.GetCluster(name);

        public IKafkaCluster GetCluster(string name) =>
            this.clusters.TryGetValue(name, out var cluster) ? cluster : null;

        void IClusterAccessor.Add(string name, IKafkaCluster cluster) => this.clusters.Add(name, cluster);
    }
}
