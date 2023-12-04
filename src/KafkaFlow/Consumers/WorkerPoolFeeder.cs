using System;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaFlow.Consumers;

internal class WorkerPoolFeeder : IWorkerPoolFeeder
{
    private readonly IConsumer _consumer;
    private readonly IConsumerWorkerPool _workerPool;
    private readonly ILogHandler _logHandler;

    private CancellationTokenSource _stopTokenSource;
    private Task _feederTask;

    public WorkerPoolFeeder(
        IConsumer consumer,
        IConsumerWorkerPool workerPool,
        ILogHandler logHandler)
    {
        _consumer = consumer;
        _workerPool = workerPool;
        _logHandler = logHandler;
    }

    public void Start()
    {
        _stopTokenSource = new CancellationTokenSource();
        var token = _stopTokenSource.Token;

        _feederTask = Task.Run(
            async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var message = await _consumer
                            .ConsumeAsync(token)
                            .ConfigureAwait(false);

                        if (message is null)
                        {
                            continue;
                        }

                        await _workerPool
                            .EnqueueAsync(message, token)
                            .ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        // Do nothing
                    }
                    catch (Exception ex)
                    {
                        _logHandler.Error(
                            "Error consuming message from Kafka",
                            ex,
                            null);
                    }
                }
            },
            CancellationToken.None);
    }

    public async Task StopAsync()
    {
        if (_stopTokenSource is { IsCancellationRequested: false })
        {
            _stopTokenSource.Cancel();
            _stopTokenSource.Dispose();
        }

        await (_feederTask ?? Task.CompletedTask);
    }
}
