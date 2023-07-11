namespace KafkaFlow.Consumers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal interface IConsumerWorker : IWorker, IDisposable
    {
        CancellationToken StopCancellationToken { get; }

        IDependencyResolver WorkerDependencyResolver { get; }

        ValueTask EnqueueAsync(IMessageContext context, CancellationToken stopCancellationToken);

        Task StartAsync();

        Task StopAsync();
    }
}
