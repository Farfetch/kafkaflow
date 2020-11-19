namespace KafkaFlow.Sample.SchemaRegistryConsumer
{
    using System;
    using System.Threading.Tasks;
    using KafkaFlow.TypedHandler;

    public class TestMessageV2Handler : IMessageHandler<TestMessageV2>
    {
        public Task Handle(IMessageContext context, TestMessageV2 message)
        {
            Console.WriteLine("Text: {0} | Text1: {1}", message.Text, message.Text1);

            return Task.CompletedTask;
        }
    }
}
