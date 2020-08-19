namespace KafkaFlow.Admin.Handlers
{
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Consumers;
    using KafkaFlow.TypedHandler;

    internal class ResumeConsumersByGroupTopicHandler : IMessageHandler<ResumeConsumersByGroupTopic>
    {
        private readonly IConsumerAccessor consumerAccessor;

        public ResumeConsumersByGroupTopicHandler(IConsumerAccessor consumerAccessor) => this.consumerAccessor = consumerAccessor;

        public Task Handle(IMessageContext context, ResumeConsumersByGroupTopic message)
        {
            var consumers = this.consumerAccessor.All.Where(x => x.GroupId == message.GroupId);

            foreach (var consumer in consumers)
            {
                consumer.Resume(consumer.Assignment.Where(x => x.Topic == message.Topic));
            }

            return Task.CompletedTask;
        }
    }
}
