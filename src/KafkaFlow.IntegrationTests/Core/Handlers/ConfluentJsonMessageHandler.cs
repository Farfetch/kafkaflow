namespace KafkaFlow.IntegrationTests.Core.Handlers
{
    using System.Threading.Tasks;
    using KafkaFlow.TypedHandler;
    using Messages;

    public class ConfluentJsonMessageHandler : IMessageHandler<TestMessage3>
    {
        public Task Handle(IMessageContext context, TestMessage3 message)
        {
            MessageStorage.Add(message);
            return Task.CompletedTask;
        }
    }
}
