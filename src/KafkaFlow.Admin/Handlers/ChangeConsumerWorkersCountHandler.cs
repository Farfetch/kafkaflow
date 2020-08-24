namespace KafkaFlow.Admin.Handlers
{
    using System.Threading.Tasks;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Consumers;
    using KafkaFlow.TypedHandler;

    internal class ChangeConsumerWorkersCountHandler : IMessageHandler<ChangeConsumerWorkerCount>
    {
        private readonly IConsumerAccessor consumerAccessor;

        public ChangeConsumerWorkersCountHandler(IConsumerAccessor consumerAccessor) => this.consumerAccessor = consumerAccessor;

        public Task Handle(IMessageContext context, ChangeConsumerWorkerCount message)
        {
            var consumer = this.consumerAccessor[message.ConsumerName];

            return
                consumer?.ChangeWorkerCountAndRestartAsync(message.WorkerCount) ??
                Task.CompletedTask;
        }
    }
}
