namespace KafkaFlow.BatchConsume
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    internal class BatchConsumeMiddleware : IMessageMiddleware
    {
        private readonly SemaphoreSlim semaphore = new(1, 1);

        private readonly List<IMessageContext> batch;
        private readonly TimeSpan batchTimeout;
        private readonly ILogHandler logHandler;

        private Task<Task> dispatchTask;
        private CancellationTokenSource cancelScheduleTokenSource;

        public BatchConsumeMiddleware(
            int batchSize,
            TimeSpan batchTimeout,
            ILogHandler logHandler)
        {
            this.batch = new(batchSize);
            this.batchTimeout = batchTimeout;
            this.logHandler = logHandler;
        }

        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            context.ConsumerContext.ShouldStoreOffset = false;

            await this.semaphore.WaitAsync().ConfigureAwait(false);

            try
            {
                this.batch.Add(context);

                if (this.batch.Count < this.batch.Capacity)
                {
                    this.dispatchTask ??= this.ScheduleDispatch(context, next);
                    return;
                }

                this.cancelScheduleTokenSource.Cancel();
            }
            finally
            {
                this.semaphore.Release();
            }

            // stores the Task in a local variable to avoid NullReferenceException in case the schedule Task ends before the await below
            var localTask = this.dispatchTask;

            if (localTask != null)
            {
                await localTask.Unwrap().ConfigureAwait(false);
            }
        }

        private Task<Task> ScheduleDispatch(IMessageContext context, MiddlewareDelegate next)
        {
            this.cancelScheduleTokenSource = new CancellationTokenSource();

            return Task
                .Delay(this.batchTimeout, this.cancelScheduleTokenSource.Token)
                .ContinueWith(
                    async _ =>
                    {
                        await this.semaphore.WaitAsync().ConfigureAwait(false);

                        try
                        {
                            if (this.batch.Count > 0)
                            {
                                await this.Dispatch(context, next).ConfigureAwait(false);
                            }
                        }
                        finally
                        {
                            this.semaphore.Release();
                        }
                    });
        }

        private async Task Dispatch(IMessageContext context, MiddlewareDelegate next)
        {
            try
            {
                var batchContext = new BatchConsumeMessageContext(
                    context.ConsumerContext,
                    this.batch.ToList());

                await next(batchContext).ConfigureAwait(false);
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
                foreach (var messageContext in this.batch)
                {
                    messageContext.ConsumerContext.StoreOffset();
                }

                this.dispatchTask = null;
                this.batch.Clear();
            }
        }
    }
}
