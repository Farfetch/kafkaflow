namespace KafkaFlow.Admin.Handlers
{
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Consumers;
    using KafkaFlow.TypedHandler;

    internal class ResumeConsumersByGroupHandler : IMessageHandler<ResumeConsumersByGroup>
    {
        private readonly IConsumerAccessor consumerAccessor;

        public ResumeConsumersByGroupHandler(IConsumerAccessor consumerAccessor) => this.consumerAccessor = consumerAccessor;

        public Task Handle(IMessageContext context, ResumeConsumersByGroup message)
        {
            var consumers = this.consumerAccessor.All.Where(x => x.GroupId == message.GroupId);

            foreach (var consumer in consumers)
            {
                consumer.Resume(consumer.Assignment);
            }

            return Task.CompletedTask;
        }
    }
}
