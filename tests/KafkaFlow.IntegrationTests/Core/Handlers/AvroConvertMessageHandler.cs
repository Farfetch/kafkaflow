using System.Threading.Tasks;
using KafkaFlow.IntegrationTests.Core.Messages;

namespace KafkaFlow.IntegrationTests.Core.Handlers;

internal class AvroConvertMessageHandler : IMessageHandler<TestAvroConvertMessage>
{
    public Task Handle(IMessageContext context, TestAvroConvertMessage message)
    {
        MessageStorage.Add(message);
        return Task.CompletedTask;
    }
}
