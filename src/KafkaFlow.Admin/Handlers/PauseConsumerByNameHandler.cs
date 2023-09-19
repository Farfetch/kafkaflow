namespace KafkaFlow.Admin.Handlers
{
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Admin.Extensions;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Consumers;
    using KafkaFlow.Middlewares.TypedHandler;

    internal class PauseConsumerByNameHandler : IMessageHandler<PauseConsumerByName>
    {
        private readonly IConsumerAccessor consumerAccessor;

        public PauseConsumerByNameHandler(IConsumerAccessor consumerAccessor) => this.consumerAccessor = consumerAccessor;

        public Task Handle(IMessageContext context, PauseConsumerByName message)
        {
            var consumer = this.consumerAccessor[message.ConsumerName];

            var assignment = consumer.FilterAssigment(message.Topics);

            if (assignment.Any())
            {
                consumer?.Pause(assignment);
            }

            return Task.CompletedTask;
        }
    }
}
