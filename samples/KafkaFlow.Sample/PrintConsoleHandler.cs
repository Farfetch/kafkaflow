using System;
using System.Threading.Tasks;
using KafkaFlow.Middlewares.TypedHandler;

namespace KafkaFlow.Sample;

public class PrintConsoleHandler : IMessageHandler<TestMessage>
{
    public Task Handle(IMessageContext context, TestMessage message)
    {
        Console.WriteLine(
            "Partition: {0} | Offset: {1} | Message: {2}",
            context.ConsumerContext.Partition,
            context.ConsumerContext.Offset,
            message.Text);

        return Task.CompletedTask;
    }
}