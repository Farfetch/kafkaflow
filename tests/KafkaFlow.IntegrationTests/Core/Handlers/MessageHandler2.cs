using System;
using System.Threading.Tasks;
using KafkaFlow.IntegrationTests.Core.Messages;

namespace KafkaFlow.IntegrationTests.Core.Handlers
{
    internal class MessageHandler2 : IMessageHandler<TestMessage2>
    {
        public async Task Handle(IMessageContext context, TestMessage2 message)
        {
            await Task.Delay(new Random().Next(1000)).ConfigureAwait(false);

            MessageStorage.Add(message);
        }
    }
}
