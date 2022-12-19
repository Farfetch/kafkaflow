
using System;
using System.Threading.Tasks;
using KafkaFlow.TypedHandler;
using SchemaRegistry;

namespace KafkaFlow.Sample.SchemaRegistry.Handlers;

public class ProtobufMessageHandler : IMessageHandler<ProtobufLogMessage>
{
    public Task Handle(IMessageContext context, ProtobufLogMessage message)
    {
        Console.WriteLine(
            "Partition: {0} | Offset: {1} | Message: {2} | Protobuf",
            context.ConsumerContext.Partition,
            context.ConsumerContext.Offset,
            message.Message);

        return Task.CompletedTask;
    }
}