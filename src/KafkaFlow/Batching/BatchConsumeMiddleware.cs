using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KafkaFlow.Configuration;
using KafkaFlow.Consumers;

namespace KafkaFlow.Batching;

internal class BatchConsumeMiddleware : IMessageMiddleware, IDisposable
{
    private readonly SemaphoreSlim _dispatchSemaphore = new(1, 1);

    private readonly int _batchSize;
    private readonly TimeSpan _batchTimeout;
    private readonly ILogHandler _logHandler;
    private readonly IConsumerConfiguration _consumerConfiguration;

    private readonly List<IMessageContext> _batch;
    private CancellationTokenSource _dispatchTokenSource;
    private Task<Task> _dispatchTask;

    public BatchConsumeMiddleware(
        IConsumerMiddlewareContext middlewareContext,
        int batchSize,
        TimeSpan batchTimeout,
        ILogHandler logHandler)
    {
        _batchSize = batchSize;
        _batchTimeout = batchTimeout;
        _logHandler = logHandler;
        _batch = new(batchSize);
        _consumerConfiguration = middlewareContext.Consumer.Configuration;

        middlewareContext.Worker.WorkerStopped.Subscribe(() => this.TriggerDispatchAndWaitAsync());
    }

    public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        await _dispatchSemaphore.WaitAsync().ConfigureAwait(false);

        try
        {
            context.ConsumerContext.AutoMessageCompletion = false;

            _batch.Add(context);

            if (_batch.Count == 1)
            {
                this.ScheduleExecution(context, next);
                return;
            }
        }
        finally
        {
            _dispatchSemaphore.Release();
        }

        if (_batch.Count >= _batchSize)
        {
            await this.TriggerDispatchAndWaitAsync().ConfigureAwait(false);
        }
    }

    public void Dispose()
    {
        _dispatchTask?.Dispose();
        _dispatchTokenSource?.Dispose();
        _dispatchSemaphore.Dispose();
    }

    private async Task TriggerDispatchAndWaitAsync()
    {
        await _dispatchSemaphore.WaitAsync().ConfigureAwait(false);
        _dispatchTokenSource?.Cancel();
        _dispatchSemaphore.Release();

        await (_dispatchTask ?? Task.CompletedTask).ConfigureAwait(false);
    }

    private void ScheduleExecution(IMessageContext context, MiddlewareDelegate next)
    {
        _dispatchTokenSource = CancellationTokenSource.CreateLinkedTokenSource(context.ConsumerContext.WorkerStopped);

        _dispatchTask = Task
            .Delay(_batchTimeout, _dispatchTokenSource.Token)
            .ContinueWith(
                _ => this.DispatchAsync(context, next),
                CancellationToken.None);
    }

    private async Task DispatchAsync(IMessageContext context, MiddlewareDelegate next)
    {
        await _dispatchSemaphore.WaitAsync().ConfigureAwait(false);

        _dispatchTokenSource.Dispose();
        _dispatchTokenSource = null;

        var localBatch = _batch.ToList();

        try
        {
            if (!localBatch.Any())
            {
                return;
            }

            var batchContext = new BatchConsumeMessageContext(context.ConsumerContext, localBatch, _consumerConfiguration.ClusterConfiguration.Brokers);

            await next(batchContext).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (context.ConsumerContext.WorkerStopped.IsCancellationRequested)
        {
            foreach (var messageContext in localBatch)
            {
                messageContext.ConsumerContext.ShouldStoreOffset = false;
            }
        }
        catch (Exception ex)
        {
            _logHandler.Error(
                "Error executing a message batch",
                ex,
                new
                {
                    context.ConsumerContext.Topic,
                    context.ConsumerContext.GroupId,
                    context.ConsumerContext.WorkerId,
                });
        }
        finally
        {
            _batch.Clear();
            _dispatchSemaphore.Release();

            if (_consumerConfiguration.AutoMessageCompletion)
            {
                foreach (var messageContext in localBatch)
                {
                    messageContext.ConsumerContext.Complete();
                }
            }
        }
    }
}
