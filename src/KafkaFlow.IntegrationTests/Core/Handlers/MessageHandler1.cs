namespace KafkaFlow.IntegrationTests.Core.Handlers
{
    using System.Threading.Tasks;
    using KafkaFlow.TypedHandler;
    using Messages;

    public class MessageHandler1 : IMessageHandler<TestMessage1>
    {
        public Task Handle(IMessageContext context, TestMessage1 message)
        {
            MessageStorage.Add(message);
            return Task.CompletedTask;
        }
    }
}
