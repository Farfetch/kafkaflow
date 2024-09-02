using Confluent.Kafka;

namespace KafkaFlow.Extensions;

/// <summary>
/// Strategy extension methods.
/// </summary>
public static class StrategyExtensions
{
    /// <summary>
    /// Determine if the strategy is a "stop the world" behavior.
    /// </summary>
    /// <param name="strategy">Strategy</param>
    /// <returns></returns>
    public static bool IsStopTheWorldStrategy(this PartitionAssignmentStrategy? strategy) =>
        strategy is null or PartitionAssignmentStrategy.Range or PartitionAssignmentStrategy.RoundRobin;
}
