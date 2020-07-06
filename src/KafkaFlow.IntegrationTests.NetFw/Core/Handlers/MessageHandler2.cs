namespace KafkaFlow.IntegrationTests.NetFw.Core.Handlers
{
    using System;
    using System.Threading.Tasks;
    using Messages;
    using TypedHandler;

    public class MessageHandler2 : IMessageHandler<TestMessage2>
    {
        public Task Handle(IMessageContext context, TestMessage2 message)
        {
            MessageStorage.Add(message);
            return Task.CompletedTask;
        }
    }
}
