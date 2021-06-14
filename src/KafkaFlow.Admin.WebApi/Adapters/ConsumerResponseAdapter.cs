namespace KafkaFlow.Admin.WebApi.Adapters
{
    using System;
    using System.Linq;
    using KafkaFlow.Admin.WebApi.Contracts;
    using KafkaFlow.Consumers;

    internal static class ConsumerResponseAdapter
    {
        internal static ConsumerResponse Adapt(this IMessageConsumer consumer)
        {
            return new ConsumerResponse
            {
                Subscription = consumer.Subscription,
                ConsumerName = consumer.ConsumerName,
                GroupId = consumer.GroupId,
                FlowStatus = consumer.FlowStatus?.ToString() ?? ConsumerFlowStatus.NotRunning.ToString(),
                MemberId = consumer.MemberId,
                WorkersCount = consumer.WorkersCount,
                ClientInstanceName = consumer.ClientInstanceName,
                ManagementDisabled = consumer.ManagementDisabled,
                PartitionAssignments = consumer.PausedPartitions
                    .GroupBy(c => c.Topic)
                    .Select(c => new PartitionAssignment()
                    {
                        Topic = c.Key,
                        InstanceName = Environment.MachineName,
                        PausedPartitions = c.Select(x => x.Partition.Value),
                        LastUpdate = DateTime.Now,
                    })
                    .Union(consumer.RunningPartitions
                        .GroupBy(c => c.Topic)
                        .Select(c => new PartitionAssignment()
                        {
                            Topic = c.Key,
                            InstanceName = Environment.MachineName,
                            RunningPartitions = c.Select(x => x.Partition.Value),
                            LastUpdate = DateTime.Now,
                        })),
            };
        }
    }
}
