using System;
using System.Threading.Tasks;
using SchemaRegistry;

namespace KafkaFlow.Sample.SchemaRegistry.Handlers;

public class AvroConvertMessageHandler : IMessageHandler<AvroConvertLogMessage>
{
    public Task Handle(IMessageContext context, AvroConvertLogMessage message)
    {
        Console.WriteLine(
            "Partition: {0} | Offset: {1} | Message: {2} | Avro",
            context.ConsumerContext.Partition,
            context.ConsumerContext.Offset,
            message.Message);

        return Task.CompletedTask;
    }
}
