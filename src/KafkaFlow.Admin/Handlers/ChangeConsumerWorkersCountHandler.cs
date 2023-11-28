using System.Threading.Tasks;
using KafkaFlow.Admin.Messages;
using KafkaFlow.Consumers;

namespace KafkaFlow.Admin.Handlers
{
    internal class ChangeConsumerWorkersCountHandler : IMessageHandler<ChangeConsumerWorkersCount>
    {
        private readonly IConsumerAccessor _consumerAccessor;

        public ChangeConsumerWorkersCountHandler(IConsumerAccessor consumerAccessor) => _consumerAccessor = consumerAccessor;

        public Task Handle(IMessageContext context, ChangeConsumerWorkersCount message)
        {
            var consumer = _consumerAccessor[message.ConsumerName];

            return
                consumer?.ChangeWorkersCountAndRestartAsync(message.WorkersCount) ??
                Task.CompletedTask;
        }
    }
}
