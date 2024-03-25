using System.Threading.Tasks;

namespace KafkaFlow.IntegrationTests.Core.Handlers;

internal class NullMessageHandler : IMessageHandler<byte[]>
{
    public Task Handle(IMessageContext context, byte[] message)
    {
        MessageStorage.AddNullMessage(message);
        return Task.CompletedTask;
    }
}