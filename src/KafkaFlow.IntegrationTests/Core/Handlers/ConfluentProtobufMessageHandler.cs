namespace KafkaFlow.IntegrationTests.Core.Handlers
{
    using System.Threading.Tasks;
    using KafkaFlow.IntegrationTests.Core.Messages;
    using KafkaFlow.TypedHandler;

    internal class ConfluentProtobufMessageHandler : IMessageHandler<TestProtoMessage>
    {
        public Task Handle(IMessageContext context, TestProtoMessage message)
        {
            MessageStorage.Add(message);
            return Task.CompletedTask;
        }
    }
}
