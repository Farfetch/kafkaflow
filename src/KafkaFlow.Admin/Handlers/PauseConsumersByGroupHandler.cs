namespace KafkaFlow.Admin.Handlers
{
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Consumers;
    using KafkaFlow.TypedHandler;

    internal class PauseConsumersByGroupHandler : IMessageHandler<PauseConsumersByGroup>
    {
        private readonly IConsumerAccessor consumerAccessor;

        public PauseConsumersByGroupHandler(IConsumerAccessor consumerAccessor) => this.consumerAccessor = consumerAccessor;

        public Task Handle(IMessageContext context, PauseConsumersByGroup message)
        {
            var consumers = this.consumerAccessor.All.Where(x => x.GroupId == message.GroupId);

            foreach (var consumer in consumers)
            {
                consumer.Pause(consumer.Assignment);
            }

            return Task.CompletedTask;
        }
    }
}
