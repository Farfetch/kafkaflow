using System;
using System.Linq;
using System.Threading.Tasks;
using KafkaFlow.BatchConsume;

namespace KafkaFlow.Sample.BatchOperations;

using System.Collections.Generic;
using System.Threading.Channels;
using KafkaFlow.Consumers;
using KafkaFlow.Observer;

internal class PrintConsoleMiddleware : IMessageMiddleware, ISubjectObserver<WorkerStoppedSubject>
{
    private readonly Channel<IReadOnlyCollection<IMessageContext>> buffer = Channel.CreateBounded<IReadOnlyCollection<IMessageContext>>(5);
    private readonly Task task;

    public PrintConsoleMiddleware(IWorkerLifetimeContext workerLifetimeContext)
    {
        workerLifetimeContext.Worker.WorkerStopped.Subscribe(this);
        this.task = CreateTask();
    }

    private async Task CreateTask()
    {
        await foreach (var batch in this.buffer.Reader.ReadAllAsync())
        {
            foreach (var context in batch)
            {
                Console.WriteLine(((SampleBatchMessage)context.Message.Value).Text);

                context.ConsumerContext.StoreOffset();

                await Task.Delay(100);
            }
        }
    }

    public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        var batch = context.GetMessagesBatch();

        await this.buffer.Writer.WriteAsync(batch);
    }

    public async Task OnNotification()
    {
        this.buffer.Writer.Complete();
        await this.task;
    }
}
