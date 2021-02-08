namespace KafkaFlow.Sample
{
    using System;
    using System.Threading.Tasks;
    using KafkaFlow.TypedHandler;
    using MessageTypes;

    public class AvroMessageHandler1 : IMessageHandler<LogMessages1>
    {
        public Task Handle(IMessageContext context, LogMessages1 message)
        {
            Console.WriteLine(
                "Partition: {0} | Offset: {1} | Message: {2}",
                context.Partition,
                context.Offset,
                message.Severity.ToString());

            return Task.CompletedTask;
        }
    }
}
