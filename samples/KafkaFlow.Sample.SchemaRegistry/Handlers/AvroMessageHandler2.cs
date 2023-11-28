using System;
using System.Threading.Tasks;
using global::SchemaRegistry;

namespace KafkaFlow.Sample.SchemaRegistry.Handlers;

public class AvroMessageHandler2 : IMessageHandler<AvroLogMessage2>
{
    public Task Handle(IMessageContext context, AvroLogMessage2 message)
    {
        Console.WriteLine(
            "Partition: {0} | Offset: {1} | Message: {2} | Avro",
            context.ConsumerContext.Partition,
            context.ConsumerContext.Offset,
            message.Message);

        return Task.CompletedTask;
    }
}