namespace KafkaFlow.Admin.Handlers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Consumers;
    using KafkaFlow.TypedHandler;

    internal class ResetConsumerOffsetHandler : IMessageHandler<ResetConsumerOffset>
    {
        private readonly IConsumerAccessor consumerAccessor;

        public ResetConsumerOffsetHandler(IConsumerAccessor consumerAccessor) => this.consumerAccessor = consumerAccessor;

        public Task Handle(IMessageContext context, ResetConsumerOffset message)
        {
            var consumer = this.consumerAccessor[message.ConsumerName];

            if (consumer is null)
            {
                return Task.CompletedTask;
            }

            var offsets = consumer.Assignment
                .Select(x => new TopicPartitionOffset(x, 0))
                .ToList();

            return consumer.OverrideOffsetsAndRestartAsync(offsets);
        }
    }
}
