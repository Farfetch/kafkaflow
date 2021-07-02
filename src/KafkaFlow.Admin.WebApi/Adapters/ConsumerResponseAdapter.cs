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
            return new()
            {
                Subscription = consumer.Subscription,
                ConsumerName = consumer.ConsumerName,
                GroupId = consumer.GroupId,
                Status = consumer.Status.ToString(),
                MemberId = consumer.MemberId,
                WorkersCount = consumer.WorkersCount,
                ClientInstanceName = consumer.ClientInstanceName,
                ManagementDisabled = consumer.ManagementDisabled,
            };
        }
    }
}
