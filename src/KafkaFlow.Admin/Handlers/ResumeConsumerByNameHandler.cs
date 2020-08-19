namespace KafkaFlow.Admin.Handlers
{
    using System.Threading.Tasks;
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

            consumer?.Resume(consumer.Assignment);

            return Task.CompletedTask;
        }
    }
}
