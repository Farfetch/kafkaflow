namespace KafkaFlow.BatchConsume
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using KafkaFlow.Observer;

    internal class BatchConsumeMiddleware
        : IMessageMiddleware,
            ISubjectObserver<WorkerStoppedSubject>,
            IDisposable
    {
        private readonly SemaphoreSlim dispatchSemaphore = new(1, 1);

        private readonly int batchSize;
        private readonly TimeSpan batchTimeout;
        private readonly ILogHandler logHandler;

        private readonly List<IMessageContext> batch;
        private CancellationTokenSource dispatchTokenSource;
        private Task<Task> dispatchTask;

        public BatchConsumeMiddleware(
            IWorkerLifetimeContext workerContext,
            int batchSize,
            TimeSpan batchTimeout,
            ILogHandler logHandler)
        {
            this.batchSize = batchSize;
            this.batchTimeout = batchTimeout;
            this.logHandler = logHandler;
            this.batch = new(batchSize);

            workerContext.Worker.WorkerStopped.Subscribe(this);
        }

        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            await this.dispatchSemaphore.WaitAsync();

            try
            {
                context.ConsumerContext.ShouldStoreOffset = false;
                context.ConsumerContext.WorkerStopped.ThrowIfCancellationRequested();

                this.batch.Add(context);

                if (this.batch.Count == 1)
                {
                    this.dispatchTokenSource = CancellationTokenSource.CreateLinkedTokenSource(context.ConsumerContext.WorkerStopped);

                    this.dispatchTokenSource.CancelAfter(this.batchTimeout);

                    this.dispatchTask = Task
                        .Delay(Timeout.Infinite, this.dispatchTokenSource.Token)
                        .ContinueWith(
                            async _ =>
                            {
                                this.dispatchTokenSource.Dispose();
                                await this.DispatchAsync(context, next);
                            });
                }

                if (this.batch.Count >= this.batchSize)
                {
                    this.dispatchTokenSource.Cancel();
                }
            }
            finally
            {
                this.dispatchSemaphore.Release();
            }
        }

        async Task ISubjectObserver<WorkerStoppedSubject>.OnNotification()
        {
            this.dispatchTokenSource?.Cancel();
            await (this.dispatchTask ?? Task.CompletedTask);
        }

        public void Dispose()
        {
            this.dispatchTask?.Dispose();
            this.dispatchTokenSource?.Dispose();
            this.dispatchSemaphore.Dispose();
        }

        private async Task DispatchAsync(IMessageContext context, MiddlewareDelegate next)
        {
            await this.dispatchSemaphore.WaitAsync();
            var localBatch = this.batch.ToList();

            try
            {
                if (!localBatch.Any())
                {
                    return;
                }

                var batchContext = new BatchConsumeMessageContext(context.ConsumerContext, localBatch);

                await next(batchContext).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (context.ConsumerContext.WorkerStopped.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                this.logHandler.Error(
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
                this.batch.Clear();
                this.dispatchSemaphore.Release();
            }

            foreach (var messageContext in localBatch)
            {
                messageContext.ConsumerContext.StoreOffset();
            }
        }
    }
}
