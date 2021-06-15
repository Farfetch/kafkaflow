namespace KafkaFlow.Admin.WebApi.Adapters
{
    using System.Collections.Generic;
    using System.Linq;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Admin.WebApi.Contracts;

    internal static class TelemetryResponseAdapter
    {
        internal static TelemetryResponse Adapt(this IEnumerable<ConsumerMetric> metrics)
        {
            return new TelemetryResponse
            {
                Groups = metrics
                    .GroupBy(metric => metric.GroupId)
                    .Select(
                        groupedMetric => new TelemetryResponse.ConsumerGroup
                        {
                            GroupId = groupedMetric.First().GroupId,
                            Consumers = groupedMetric
                                .Select(x => x)
                                .GroupBy(x => x.ConsumerName)
                                .Select(
                                    metric => new TelemetryResponse.Consumer
                                    {
                                        Name = metric.First().ConsumerName,
                                        WorkersCount = metric.First().WorkersCount,
                                        Assignments = metric.Select(
                                            x => new TelemetryResponse.TopicPartitionAssignment
                                            {
                                                InstanceName = x.InstanceName,
                                                TopicName = x.Topic,
                                                Status = x.Status.ToString(),
                                                LastUpdate = x.SentAt,
                                                PausedPartitions = x.PausedPartitions,
                                                RunningPartitions = x.RunningPartitions,
                                            }),
                                    }),
                        }),
            };
        }
    }
}
