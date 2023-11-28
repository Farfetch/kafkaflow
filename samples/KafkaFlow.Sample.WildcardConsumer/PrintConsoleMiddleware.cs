using System.Text;
using KafkaFlow;

public class PrintConsoleMiddleware : IMessageMiddleware
{
    public Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        Console.WriteLine(
            "Topic: {0} | Partition: {1} | Offset: {2} | Message: {3}",
            context.ConsumerContext.Topic,
            context.ConsumerContext.Partition,
            context.ConsumerContext.Offset,
            Encoding.UTF8.GetString(
                (byte[])context.Message.Value));

        return next(context);
    }
}