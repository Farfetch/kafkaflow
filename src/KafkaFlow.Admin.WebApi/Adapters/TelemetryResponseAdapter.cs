using System.Collections.Generic;
using System.Linq;
using KafkaFlow.Admin.Messages;
using KafkaFlow.Admin.WebApi.Contracts;

namespace KafkaFlow.Admin.WebApi.Adapters;

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
                                        WorkersCount = consumerMetricsList
                                            .OrderByDescending(x => x.SentAt)
                                            .First()
                                            .WorkersCount,
                                        Assignments = consumerMetricsList
                                            .OrderBy(x => x.Topic)
                                            .Select(
                                                m => new TelemetryResponse.TopicPartitionAssignment
                                                {
                                                    InstanceName = m.InstanceName,
                                                    TopicName = m.Topic,
                                                    Status = m.Status.ToString(),
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
