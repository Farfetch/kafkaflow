using System.Linq;
using System.Threading.Tasks;
using KafkaFlow.Admin.Extensions;
using KafkaFlow.Admin.Messages;
using KafkaFlow.Consumers;

namespace KafkaFlow.Admin.Handlers
{
    internal class PauseConsumerByNameHandler : IMessageHandler<PauseConsumerByName>
    {
        private readonly IConsumerAccessor _consumerAccessor;

        public PauseConsumerByNameHandler(IConsumerAccessor consumerAccessor) => _consumerAccessor = consumerAccessor;

        public Task Handle(IMessageContext context, PauseConsumerByName message)
        {
            var consumer = _consumerAccessor[message.ConsumerName];

            var assignment = consumer.FilterAssigment(message.Topics);

            if (assignment.Any())
            {
                consumer?.Pause(assignment);
            }

            return Task.CompletedTask;
        }
    }
}
