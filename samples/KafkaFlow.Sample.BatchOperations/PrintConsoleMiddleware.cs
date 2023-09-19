using System;
using System.Linq;
using System.Threading.Tasks;
using KafkaFlow.Batching;

namespace KafkaFlow.Sample.BatchOperations;

internal class PrintConsoleMiddleware : IMessageMiddleware
{
    public Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        var batch = context.GetMessagesBatch();

        var text = string.Join(
            '\n',
            batch.Select(ctx => ((SampleBatchMessage) ctx.Message.Value).Text));

        Console.WriteLine(text);

        return Task.CompletedTask;
    }
}