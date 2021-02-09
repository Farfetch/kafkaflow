namespace KafkaFlow.Admin.Handlers
{
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Consumers;
    using KafkaFlow.TypedHandler;

    internal class PauseConsumersByGroupTopicHandler : IMessageHandler<PauseConsumersByGroupTopic>
    {
        private readonly IConsumerAccessor consumerAccessor;

        public PauseConsumersByGroupTopicHandler(IConsumerAccessor consumerAccessor) => this.consumerAccessor = consumerAccessor;

        public Task Handle(IMessageContext context, PauseConsumersByGroupTopic message)
        {
            var consumers = this.consumerAccessor.All
                .Where(x => x.GroupId == message.GroupId);

            foreach (var consumer in consumers)
            {
                consumer.Pause(consumer.Assignment
                    .Where(x => x.Topic == message.Topic)
                    .ToList());
            }

            return Task.CompletedTask;
        }
    }
}
