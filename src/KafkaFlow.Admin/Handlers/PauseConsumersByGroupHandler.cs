using System.Linq;
using System.Threading.Tasks;
using KafkaFlow.Admin.Extensions;
using KafkaFlow.Admin.Messages;
using KafkaFlow.Consumers;

namespace KafkaFlow.Admin.Handlers
{
    internal class PauseConsumersByGroupHandler : IMessageHandler<PauseConsumersByGroup>
    {
        private readonly IConsumerAccessor _consumerAccessor;

        public PauseConsumersByGroupHandler(IConsumerAccessor consumerAccessor) => _consumerAccessor = consumerAccessor;

        public Task Handle(IMessageContext context, PauseConsumersByGroup message)
        {
            var consumers = _consumerAccessor.All.Where(x => x.GroupId == message.GroupId);

            foreach (var consumer in consumers)
            {
                var assignment = consumer.FilterAssigment(message.Topics);

                if (assignment.Any())
                {
                    consumer.Pause(assignment);
                }
            }

            return Task.CompletedTask;
        }
    }
}
