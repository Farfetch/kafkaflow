namespace KafkaFlow.IntegrationTests.Core.Handlers
{
    using System.Threading.Tasks;
    using KafkaFlow.TypedHandler;
    using Messages;
    using MessageTypes;

    public class AvroMessageHandler : IMessageHandler<LogMessages2>
    {
        public Task Handle(IMessageContext context, LogMessages2 message)
        {
            MessageStorage.Add(message);
            return Task.CompletedTask;
        }
    }
}
