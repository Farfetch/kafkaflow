namespace KafkaFlow.IntegrationTests.Common.Core.Handlers
{
    using System.Threading.Tasks;
    using KafkaFlow.TypedHandler;
    using Messages;

    public class MessageHandler2 : IMessageHandler<TestMessage2>
    {
        public async Task Handle(IMessageContext context, TestMessage2 message)
        {
            MessageStorage.Add(message);
        }
    }
}
