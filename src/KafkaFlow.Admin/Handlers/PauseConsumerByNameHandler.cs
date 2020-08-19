namespace KafkaFlow.Admin.Handlers
{
    using System.Threading.Tasks;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Consumers;
    using KafkaFlow.TypedHandler;

    internal class PauseConsumerByNameHandler : IMessageHandler<PauseConsumerByName>
    {
        private readonly IConsumerAccessor consumerAccessor;

        public PauseConsumerByNameHandler(IConsumerAccessor consumerAccessor) => this.consumerAccessor = consumerAccessor;

        public Task Handle(IMessageContext context, PauseConsumerByName message)
        {
            var consumer = this.consumerAccessor[message.ConsumerName];

            consumer?.Pause(consumer.Assignment);

            return Task.CompletedTask;
        }
    }
}
