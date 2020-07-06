namespace KafkaFlow.IntegrationTests.NetFw.Core.Handlers
{
    using System.Threading.Tasks;
    using Messages;
    using TypedHandler;

    public class MessageHandler : IMessageHandler<TestMessage1>
    {
        public Task Handle(IMessageContext context, TestMessage1 message)
        {
            MessageStorage.Add(message);
            return Task.CompletedTask;
        }
    }
}
