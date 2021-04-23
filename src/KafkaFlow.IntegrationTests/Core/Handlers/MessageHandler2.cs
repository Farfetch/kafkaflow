namespace KafkaFlow.IntegrationTests.Core.Handlers
{
    using System;
    using System.Threading.Tasks;
    using KafkaFlow.IntegrationTests.Core.Messages;
    using KafkaFlow.TypedHandler;

    internal class MessageHandler2 : IMessageHandler<TestMessage2>
    {
        public async Task Handle(IMessageContext context, TestMessage2 message)
        {
            await Task.Delay(new Random().Next(1000)).ConfigureAwait(false);

            MessageStorage.Add(message);
        }
    }
}
