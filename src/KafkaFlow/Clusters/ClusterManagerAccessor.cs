namespace KafkaFlow.Clusters
{
    using System.Collections.Generic;
    using System.Linq;

    internal class ClusterManagerAccessor : IClusterManagerAccessor
    {
        private readonly Dictionary<string, IClusterManager> managers;

        public ClusterManagerAccessor(IEnumerable<IClusterManager> managers)
        {
            this.managers = managers.ToDictionary(manager => manager.ClusterName);
        }

        public IEnumerable<IClusterManager> All => this.managers.Values;

        public IClusterManager this[string name] => this.GetManager(name);

        public IClusterManager GetManager(string name) =>
            this.managers.TryGetValue(name, out var manager) ? manager : null;
    }
}
