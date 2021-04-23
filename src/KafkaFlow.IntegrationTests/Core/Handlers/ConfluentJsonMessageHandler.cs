namespace KafkaFlow.IntegrationTests.Core.Handlers
{
    using System.Threading.Tasks;
    using KafkaFlow.IntegrationTests.Core.Messages;
    using KafkaFlow.TypedHandler;

    internal class ConfluentJsonMessageHandler : IMessageHandler<TestMessage3>
    {
        public Task Handle(IMessageContext context, TestMessage3 message)
        {
            MessageStorage.Add(message);
            return Task.CompletedTask;
        }
    }
}
