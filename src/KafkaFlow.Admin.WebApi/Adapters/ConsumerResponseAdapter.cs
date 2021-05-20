namespace KafkaFlow.Admin.WebApi.Adapters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Admin.WebApi.Contracts;
    using KafkaFlow.Consumers;
    using Microsoft.Extensions.Caching.Memory;

    internal static class ConsumerResponseAdapter
    {
        internal static ConsumerResponse Adapt(this IMessageConsumer consumer, IMemoryCache cache)
        {
            var consumerResponse = new ConsumerResponse()
            {
                Subscription = consumer.Subscription,
                IsReadonly = consumer.IsReadonly,
                ConsumerName = consumer.ConsumerName,
                GroupId = consumer.GroupId,
                FlowStatus = consumer.FlowStatus ?? ConsumerFlowStatus.NotRunning,
                MemberId = consumer.MemberId,
                WorkersCount = consumer.WorkersCount,
                ClientInstanceName = consumer.ClientInstanceName,
                IsReadonly = consumer.IsReadonly,
            };

            consumerResponse.PartitionAssignments =
                cache.TryGetValue($"{consumer.GroupId}-{consumer.ConsumerName}", out List<ConsumerMetric> metric) ?
                    metric.Select(m => m.Adapt()) :
                    consumer.GetLocalPartitionAssignments();

            return consumerResponse;
        }

        private static IEnumerable<PartitionAssignment> GetLocalPartitionAssignments(this IMessageConsumer consumer)
        {
            return consumer.PausedPartitions
                .GroupBy(c => c.Topic)
                .Select(c => new PartitionAssignment()
                {
                    Topic = c.Key,
                    HostName = Environment.MachineName,
                    PausedPartitions = c.Select(x => x.Partition.Value).ToList(),
                    LastUpdate = DateTime.Now,
                })
                .Union(consumer.RunningPartitions
                    .GroupBy(c => c.Topic)
                    .Select(c => new PartitionAssignment()
                    {
                        Topic = c.Key,
                        HostName = Environment.MachineName,
                        RunningPartitions = c.Select(x => x.Partition.Value).ToList(),
                        LastUpdate = DateTime.Now,
                    }));
        }

        private static PartitionAssignment Adapt(this ConsumerMetric consumer) =>
            new()
            {
                Topic = consumer.Topic,
                HostName = consumer.HostName,
                PausedPartitions = consumer.PausedPartitions,
                RunningPartitions = consumer.RunningPartitions,
                LastUpdate = consumer.SentAt,
            };
    }
}
