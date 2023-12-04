using System.Threading.Tasks;
using KafkaFlow.IntegrationTests.Core.Messages;

namespace KafkaFlow.IntegrationTests.Core.Handlers;

internal class MessageHandler1 : IMessageHandler<TestMessage1>
{
    public Task Handle(IMessageContext context, TestMessage1 message)
    {
        MessageStorage.Add(message);
        return Task.CompletedTask;
    }
}
