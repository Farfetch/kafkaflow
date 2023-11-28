using System;
using System.Threading.Tasks;
using global::SchemaRegistry;

namespace KafkaFlow.Sample.SchemaRegistry.Handlers
{
    public class AvroMessageHandler : IMessageHandler<AvroLogMessage>
    {
        public Task Handle(IMessageContext context, AvroLogMessage message)
        {
            Console.WriteLine(
                "Partition: {0} | Offset: {1} | Message: {2} | Avro",
                context.ConsumerContext.Partition,
                context.ConsumerContext.Offset,
                message.Severity.ToString());

            return Task.CompletedTask;
        }
    }
}