namespace KafkaFlow.Clusters
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides access to the configured cluster manager
    /// </summary>
    internal interface IClusterManagerAccessor
    {
        /// <summary>
        /// Gets all configured cluster managers
        /// </summary>
        IEnumerable<IClusterManager> All { get; }

        /// <summary>
        /// Gets a cluster manager by its name
        /// </summary>
        /// <param name="name">cluster name</param>
        IClusterManager this[string name] { get; }

        /// <summary>
        /// Gets a cluster manager by its name
        /// </summary>
        /// <param name="name">The name defined in the cluster configuration</param>
        /// <returns></returns>
        IClusterManager GetManager(string name);
    }
}
