using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace KafkaFlow.Configuration;

/// <summary>
/// Represents a Topic configuration
/// </summary>
public class TopicConfiguration
{
    private static readonly IReadOnlyDictionary<string, string> s_emptyConfigs = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());
    private static readonly IReadOnlyDictionary<int, IReadOnlyList<int>> s_emptyReplicasAssignments = new ReadOnlyDictionary<int, IReadOnlyList<int>>(new Dictionary<int, IReadOnlyList<int>>());

    /// <summary>
    /// Initializes a new instance of the <see cref="TopicConfiguration"/> class.
    /// </summary>
    /// <param name="name">The topic name</param>
    /// <param name="partitions">The number of partitions for the topic</param>
    /// <param name="replicas">Replication factor for the topic</param>
    /// <param name="configs">The configuration to use to create the new topic</param>
    /// <param name="replicasAssignments">A map from partition id to replica ids</param>
    public TopicConfiguration(string name, int partitions, short replicas, IDictionary<string, string> configs = null, IDictionary<int, IEnumerable<int>> replicasAssignments = null)
    {
        this.Name = name;
        this.Partitions = partitions;
        this.Replicas = replicas;
        this.Configs = configs is null ? s_emptyConfigs : new ReadOnlyDictionary<string, string>(configs);
        this.ReplicasAssignments = replicasAssignments is null
            ? s_emptyReplicasAssignments
            : new ReadOnlyDictionary<int, IReadOnlyList<int>>(replicasAssignments.ToDictionary(x => x.Key, x => (IReadOnlyList<int>)x.Value.ToList()));
    }

    /// <summary>
    /// Gets the Topic Name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the number of Topic Partitions
    /// </summary>
    public int Partitions { get; }

    /// <summary>
    /// Gets the Topic Replication Factor
    /// </summary>
    public short Replicas { get; }

    /// <summary>Gets the configuration to use to create the new topic.</summary>
    public IReadOnlyDictionary<string, string> Configs { get; }

    /// <summary>
    ///     Gets the mapping from partition id to replica ids (i.e., static broker ids) or null
    ///     if the number of partitions and replication factor are specified
    ///     instead.
    /// </summary>
    public IReadOnlyDictionary<int, IReadOnlyList<int>> ReplicasAssignments { get; }
}
