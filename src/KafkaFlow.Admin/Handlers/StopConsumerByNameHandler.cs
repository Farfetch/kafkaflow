using System.Threading.Tasks;
using KafkaFlow.Admin.Messages;
using KafkaFlow.Consumers;

namespace KafkaFlow.Admin.Handlers
{
    internal class StopConsumerByNameHandler : IMessageHandler<StopConsumerByName>
    {
        private readonly IConsumerAccessor _consumerAccessor;

        public StopConsumerByNameHandler(IConsumerAccessor consumerAccessor) => _consumerAccessor = consumerAccessor;

        public async Task Handle(IMessageContext context, StopConsumerByName message)
        {
            var consumer = _consumerAccessor[message.ConsumerName];

            await consumer.StopAsync();
        }
    }
}
