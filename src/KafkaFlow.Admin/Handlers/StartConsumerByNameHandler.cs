namespace KafkaFlow.Admin.Handlers
{
    using System.Threading.Tasks;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Consumers;
    using KafkaFlow.TypedHandler;

    internal class StartConsumerByNameHandler : IMessageHandler<StartConsumerByName>
    {
        private readonly IConsumerAccessor consumerAccessor;

        public StartConsumerByNameHandler(IConsumerAccessor consumerAccessor) => this.consumerAccessor = consumerAccessor;

        public async Task Handle(IMessageContext context, StartConsumerByName message)
        {
            var consumer = this.consumerAccessor[message.ConsumerName];

            await consumer.StartAsync();
        }
    }
}
