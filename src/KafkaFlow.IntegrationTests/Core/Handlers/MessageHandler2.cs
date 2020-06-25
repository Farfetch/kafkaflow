namespace KafkaFlow.IntegrationTests.Core.Handlers
{
    using System;
    using System.Threading.Tasks;
    using KafkaFlow.TypedHandler;
    using Messages;

    public class MessageHandler2 : IMessageHandler<TestMessage2>
    {
        public async Task Handle(IMessageContext context, TestMessage2 message)
        {
            await Task.Delay(new Random().Next(1000));

            MessageStorage.Add(message);
        }
    }
}
