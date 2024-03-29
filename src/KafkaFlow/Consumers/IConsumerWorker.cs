using System;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaFlow.Consumers;

internal interface IConsumerWorker : IWorker, IDisposable
{
    CancellationToken StopCancellationToken { get; }

    IDependencyResolver WorkerDependencyResolver { get; }

    ValueTask EnqueueAsync(IMessageContext context);

    Task StartAsync(CancellationToken stopCancellationToken);

    Task StopAsync();
}