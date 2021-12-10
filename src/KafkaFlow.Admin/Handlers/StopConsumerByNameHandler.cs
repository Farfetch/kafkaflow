namespace KafkaFlow.Admin.Handlers
{
    using System.Threading.Tasks;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Consumers;
    using KafkaFlow.TypedHandler;

    internal class StopConsumerByNameHandler : IMessageHandler<StopConsumerByName>
    {
        private readonly IConsumerAccessor consumerAccessor;

        public StopConsumerByNameHandler(IConsumerAccessor consumerAccessor) => this.consumerAccessor = consumerAccessor;

        public async Task Handle(IMessageContext context, StopConsumerByName message)
        {
            var consumer = this.consumerAccessor[message.ConsumerName];

            await consumer.StopAsync();
        }
    }
}
