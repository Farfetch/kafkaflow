namespace KafkaFlow.Sample.SchemaRegistry.Handlers;

using System;
using System.Threading.Tasks;
using global::SchemaRegistry;
using TypedHandler;

public class JsonMessageHandler : IMessageHandler<JsonLogMessage>
{
    public Task Handle(IMessageContext context, JsonLogMessage message)
    {
        Console.WriteLine(
            "Partition: {0} | Offset: {1} | Message: {2} | Json",
            context.ConsumerContext.Partition,
            context.ConsumerContext.Offset,
            message.Message);

        return Task.CompletedTask;
    }
}