using System;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using KafkaFlow.Admin.Extensions;
using KafkaFlow.Admin.Messages;
using KafkaFlow.Consumers;

namespace KafkaFlow.Admin.Handlers
{
    internal class RewindConsumerOffsetToDateTimeHandler : IMessageHandler<RewindConsumerOffsetToDateTime>
    {
        private readonly IConsumerAccessor _consumerAccessor;

        public RewindConsumerOffsetToDateTimeHandler(IConsumerAccessor consumerAccessor) => _consumerAccessor = consumerAccessor;

        public Task Handle(IMessageContext context, RewindConsumerOffsetToDateTime message)
        {
            var consumer = _consumerAccessor[message.ConsumerName];

            if (consumer is null)
            {
                return Task.CompletedTask;
            }

            var offsets = consumer.GetOffsets(
                consumer
                    .FilterAssigment(message.Topics)
                    .Select(partition => new TopicPartitionTimestamp(partition, new Timestamp(message.DateTime))),
                TimeSpan.FromSeconds(30));

            return offsets?.Any() == true ?
                consumer.OverrideOffsetsAndRestartAsync(offsets) :
                Task.CompletedTask;
        }
    }
}
