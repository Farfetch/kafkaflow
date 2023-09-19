namespace KafkaFlow.Sample.SchemaRegistry.Handlers
{
    using System;
    using System.Threading.Tasks;
    using KafkaFlow.Middlewares.TypedHandler;
    using global::SchemaRegistry;

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