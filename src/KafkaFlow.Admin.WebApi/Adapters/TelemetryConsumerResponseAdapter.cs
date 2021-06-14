namespace KafkaFlow.Admin.WebApi.Adapters
{
    using System.Linq;
    using KafkaFlow.Admin.WebApi.Contracts;
    using KafkaFlow.Consumers;

    internal static class TelemetryConsumerResponseAdapter
    {
        internal static TelemetryConsumerResponse Adapt(this IMessageConsumer consumer, ITelemetryStorage storage)
        {
            var metrics = storage.Get(consumer.GroupId, consumer.ConsumerName);

            return new TelemetryConsumerResponse()
            {
                ConsumerName = consumer.ConsumerName,
                GroupId = consumer.GroupId,
                FlowStatus = consumer.FlowStatus?.ToString() ?? ConsumerFlowStatus.NotRunning.ToString(),
                WorkersCount = consumer.WorkersCount,
                ManagementDisabled = consumer.ManagementDisabled,
                PartitionAssignments = metrics.Select(m => new PartitionAssignment()
                {
                    Topic = m.Topic,
                    InstanceName = m.InstanceName,
                    PausedPartitions = m.PausedPartitions,
                    RunningPartitions = m.RunningPartitions,
                    LastUpdate = m.SentAt,
                }),
            };
        }
    }
}
