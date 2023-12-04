using System.Collections.Generic;
using System.Linq;
using KafkaFlow.Admin.Messages;

namespace KafkaFlow.Admin.Dashboard;

internal static class TelemetryResponseAdapter
{
    internal static TelemetryResponse Adapt(this IEnumerable<ConsumerTelemetryMetric> metrics)
    {
        return new TelemetryResponse
        {
            Groups = metrics
                .OrderBy(x => x.GroupId)
                .GroupBy(
                    metric => metric.GroupId,
                    (groupId, groupedMetric) => new TelemetryResponse.ConsumerGroup
                    {
                        GroupId = groupId,
                        Consumers = groupedMetric
                            .OrderBy(x => x.ConsumerName)
                            .GroupBy(
                                x => x.ConsumerName,
                                (consumerName, consumerMetrics) =>
                                {
                                    var consumerMetricsList = consumerMetrics.ToList();

                                    return new TelemetryResponse.Consumer
                                    {
                                        Name = consumerName,
                                        Assignments = consumerMetricsList
                                            .OrderBy(x => x.Topic)
                                            .Select(
                                                m => new TelemetryResponse.TopicPartitionAssignment
                                                {
                                                    InstanceName = m.InstanceName,
                                                    TopicName = m.Topic,
                                                    Status = m.Status.ToString(),
                                                    Workers = m.WorkersCount,
                                                    LastUpdate = m.SentAt,
                                                    PausedPartitions = m.PausedPartitions,
                                                    RunningPartitions = m.RunningPartitions,
                                                    Lag = m.Lag,
                                                }),
                                    };
                                }),
                    }),
        };
    }
}
