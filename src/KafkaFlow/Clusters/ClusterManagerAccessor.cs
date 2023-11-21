using System.Collections.Generic;
using System.Linq;

namespace KafkaFlow.Clusters
{
    internal class ClusterManagerAccessor : IClusterManagerAccessor
    {
        private readonly Dictionary<string, IClusterManager> _managers;

        public ClusterManagerAccessor(IEnumerable<IClusterManager> managers)
        {
            _managers = managers.ToDictionary(manager => manager.ClusterName);
        }

        public IEnumerable<IClusterManager> All => _managers.Values;

        public IClusterManager this[string name] => this.GetManager(name);

        public IClusterManager GetManager(string name) =>
            _managers.TryGetValue(name, out var manager) ? manager : null;
    }
}
