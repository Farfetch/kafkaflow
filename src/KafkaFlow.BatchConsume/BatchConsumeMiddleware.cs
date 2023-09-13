namespace KafkaFlow.BatchConsume
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using KafkaFlow.Configuration;
    using KafkaFlow.Consumers;
    using KafkaFlow.Observer;

    internal class BatchConsumeMiddleware
        : IMessageMiddleware,
            ISubjectObserver<WorkerStoppedSubject, VoidObject>,
            IDisposable
    {
        private readonly SemaphoreSlim dispatchSemaphore = new(1, 1);

        private readonly int batchSize;
        private readonly TimeSpan batchTimeout;
        private readonly ILogHandler logHandler;
        private readonly IConsumerConfiguration consumerConfiguration;

        private readonly List<IMessageContext> batch;
        private CancellationTokenSource dispatchTokenSource;
        private Task<Task> dispatchTask;

        public BatchConsumeMiddleware(
            IConsumerMiddlewareContext workerContext,
            int batchSize,
            TimeSpan batchTimeout,
            ILogHandler logHandler)
        {
            this.batchSize = batchSize;
            this.batchTimeout = batchTimeout;
            this.logHandler = logHandler;
            this.batch = new(batchSize);
            this.consumerConfiguration = workerContext.Consumer.Configuration;

            workerContext.Worker.WorkerStopped.Subscribe(this);
        }

        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            await this.dispatchSemaphore.WaitAsync();

            try
            {
                context.ConsumerContext.AutoMessageCompletion = false;

                this.batch.Add(context);

                if (this.batch.Count == 1)
                {
                    this.ScheduleExecution(context, next);
                    return;
                }
            }
            finally
            {
                this.dispatchSemaphore.Release();
            }

            if (this.batch.Count >= this.batchSize)
            {
                await this.TriggerDispatchAndWaitAsync();
            }
        }

        public async Task OnNotification(WorkerStoppedSubject subject, VoidObject arg) => await this.TriggerDispatchAndWaitAsync();

        public void Dispose()
        {
            this.dispatchTask?.Dispose();
            this.dispatchTokenSource?.Dispose();
            this.dispatchSemaphore.Dispose();
        }

        private async Task TriggerDispatchAndWaitAsync()
        {
            await this.dispatchSemaphore.WaitAsync();
            this.dispatchTokenSource?.Cancel();
            this.dispatchSemaphore.Release();

            await (this.dispatchTask ?? Task.CompletedTask);
        }

        private void ScheduleExecution(IMessageContext context, MiddlewareDelegate next)
        {
            this.dispatchTokenSource = CancellationTokenSource.CreateLinkedTokenSource(context.ConsumerContext.WorkerStopped);

            this.dispatchTask = Task
                .Delay(this.batchTimeout, this.dispatchTokenSource.Token)
                .ContinueWith(
                    _ => this.DispatchAsync(context, next),
                    CancellationToken.None);
        }

        private async Task DispatchAsync(IMessageContext context, MiddlewareDelegate next)
        {
            await this.dispatchSemaphore.WaitAsync();

            this.dispatchTokenSource.Dispose();
            this.dispatchTokenSource = null;

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
                foreach (var messageContext in localBatch)
                {
                    messageContext.ConsumerContext.StoreOffsetOnCompletion = false;
                }
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

                if (this.consumerConfiguration.AutoMessageCompletion)
                {
                    foreach (var messageContext in localBatch)
                    {
                        messageContext.ConsumerContext.Complete();
                    }
                }
            }
        }
    }
}
