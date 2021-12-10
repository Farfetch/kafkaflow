namespace KafkaFlow.Admin.Handlers
{
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Admin.Extensions;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Consumers;
    using KafkaFlow.TypedHandler;

    internal class ResumeConsumerByNameHandler : IMessageHandler<ResumeConsumerByName>
    {
        private readonly IConsumerAccessor consumerAccessor;

        public ResumeConsumerByNameHandler(IConsumerAccessor consumerAccessor) => this.consumerAccessor = consumerAccessor;

        public Task Handle(IMessageContext context, ResumeConsumerByName message)
        {
            var consumer = this.consumerAccessor[message.ConsumerName];

            var assignment = consumer.FilterAssigment(message.Topics);

            if (assignment.Any())
            {
                consumer?.Resume(assignment);
            }

            return Task.CompletedTask;
        }
    }
}
