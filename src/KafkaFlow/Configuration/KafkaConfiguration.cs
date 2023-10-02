namespace KafkaFlow.Configuration;

using System.Collections.Generic;

/// <summary>
/// Represents the kafka configuration values
/// </summary>
public class KafkaConfiguration
{
    private readonly List<ClusterConfiguration> clusters = new();

    internal KafkaConfiguration(GlobalEventsConfiguration globalEventsConfiguration)
    {
        this.GlobalEventsConfiguration = globalEventsConfiguration;
    }

    /// <summary>
    /// Gets the cluster configuration list
    /// </summary>
    public IReadOnlyCollection<ClusterConfiguration> Clusters => this.clusters;

    internal GlobalEventsConfiguration GlobalEventsConfiguration { get; }

    internal void AddClusters(IEnumerable<ClusterConfiguration> configurations) =>
        this.clusters.AddRange(configurations);
}
