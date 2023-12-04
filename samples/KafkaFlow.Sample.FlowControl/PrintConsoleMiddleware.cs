using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KafkaFlow.Sample.FlowControl;

internal class PrintConsoleMiddleware : IMessageMiddleware
{
    public Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        Console.WriteLine($"Message: {JsonConvert.SerializeObject(context.Message.Value)}");
        return Task.CompletedTask;
    }
}