namespace KafkaFlow.Consumers
{
    using System.Collections.Generic;
    using KafkaFlow.Client;

    /// <summary>
    /// Provides access to the configured clusters
    /// </summary>
    public interface IClusterAccessor
    {
        /// <summary>
        /// Gets all configured clusters
        /// </summary>
        IEnumerable<IKafkaCluster> All { get; }

        /// <summary>
        /// Gets a cluster by its name
        /// </summary>
        /// <param name="name">cluster name</param>
        IKafkaCluster this[string name] { get; }

        /// <summary>
        /// Gets a cluster by its name
        /// </summary>
        /// <param name="name">The name defined in the cluster configuration</param>
        /// <returns></returns>
        IKafkaCluster GetCluster(string name);

        internal void Add(string name, IKafkaCluster cluster);
    }
}
