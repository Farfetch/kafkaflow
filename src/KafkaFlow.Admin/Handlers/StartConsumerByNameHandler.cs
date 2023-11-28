using System.Threading.Tasks;
using KafkaFlow.Admin.Messages;
using KafkaFlow.Consumers;

namespace KafkaFlow.Admin.Handlers
{
    internal class StartConsumerByNameHandler : IMessageHandler<StartConsumerByName>
    {
        private readonly IConsumerAccessor _consumerAccessor;

        public StartConsumerByNameHandler(IConsumerAccessor consumerAccessor) => _consumerAccessor = consumerAccessor;

        public async Task Handle(IMessageContext context, StartConsumerByName message)
        {
            var consumer = _consumerAccessor[message.ConsumerName];

            await consumer.StartAsync();
        }
    }
}
