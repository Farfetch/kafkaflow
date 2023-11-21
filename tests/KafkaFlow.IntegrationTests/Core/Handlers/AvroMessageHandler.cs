using System.Threading.Tasks;
using MessageTypes;

namespace KafkaFlow.IntegrationTests.Core.Handlers
{
    internal class AvroMessageHandler : IMessageHandler<LogMessages2>
    {
        public Task Handle(IMessageContext context, LogMessages2 message)
        {
            MessageStorage.Add(message);
            return Task.CompletedTask;
        }
    }
}
