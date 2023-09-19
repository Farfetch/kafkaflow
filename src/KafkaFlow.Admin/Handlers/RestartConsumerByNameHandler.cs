namespace KafkaFlow.Admin.Handlers
{
    using System.Threading.Tasks;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Consumers;
    using KafkaFlow.Middlewares.TypedHandler;

    internal class RestartConsumerByNameHandler : IMessageHandler<RestartConsumerByName>
    {
        private readonly IConsumerAccessor consumerAccessor;

        public RestartConsumerByNameHandler(IConsumerAccessor consumerAccessor) => this.consumerAccessor = consumerAccessor;

        public Task Handle(IMessageContext context, RestartConsumerByName message)
        {
            var consumer = this.consumerAccessor[message.ConsumerName];

            return consumer?.RestartAsync() ?? Task.CompletedTask;
        }
    }
}
