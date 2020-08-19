namespace KafkaFlow.Admin.Handlers
{
    using System.Threading.Tasks;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Consumers;
    using KafkaFlow.TypedHandler;

    internal class ChangeConsumerWorkersCountHandler : IMessageHandler<ChangeConsumerWorkersCount>
    {
        private readonly IConsumerAccessor consumerAccessor;

        public ChangeConsumerWorkersCountHandler(IConsumerAccessor consumerAccessor) => this.consumerAccessor = consumerAccessor;

        public Task Handle(IMessageContext context, ChangeConsumerWorkersCount message)
        {
            var consumer = this.consumerAccessor[message.ConsumerName];

            return consumer is null ?
                Task.CompletedTask :
                consumer.ChangeWorkerCountAndRestartAsync(message.WorkersCount);
        }
    }
}
