using System.Threading.Tasks;
using KafkaFlow.IntegrationTests.Core.Messages;

namespace KafkaFlow.IntegrationTests.Core.Handlers;

internal class OffsetTrackerMessageHandler : IMessageHandler<OffsetTrackerMessage>
{
    public Task Handle(IMessageContext context, OffsetTrackerMessage message)
    {
        message.Offset = context.ConsumerContext.Offset;
        MessageStorage.Add(message);
        return Task.CompletedTask;
    }
}