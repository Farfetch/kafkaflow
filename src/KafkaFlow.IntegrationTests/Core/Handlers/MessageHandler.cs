namespace KafkaFlow.IntegrationTests.Core.Handlers
{
    using System.Threading.Tasks;
    using KafkaFlow.IntegrationTests.Core.Messages;
    using KafkaFlow.Middlewares.TypedHandler;

    internal class MessageHandler : IMessageHandler<TestMessage1>
    {
        public Task Handle(IMessageContext context, TestMessage1 message)
        {
            MessageStorage.Add(message);
            return Task.CompletedTask;
        }
    }
}
