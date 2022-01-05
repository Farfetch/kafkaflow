namespace KafkaFlow.BatchConsume
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    internal class BatchConsumeMiddleware : IMessageMiddleware
    {
        private readonly int batchSize;
        private readonly TimeSpan batchTimeout;
        private readonly ILogHandler logHandler;

        private List<IMessageContext> batch;
        private Task<Task> dispatchTask;
        private CancellationTokenSource cancelScheduleTokenSource;

        public BatchConsumeMiddleware(
            int batchSize,
            TimeSpan batchTimeout,
            ILogHandler logHandler)
        {
            this.batchSize = batchSize;
            this.batchTimeout = batchTimeout;
            this.logHandler = logHandler;
            this.batch = new(batchSize);
        }

        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            context.ConsumerContext.ShouldStoreOffset = false;
            this.batch.Add(context);

            if (this.batch.Count == 1)
            {
                this.cancelScheduleTokenSource = new CancellationTokenSource();

                this.dispatchTask = Task
                    .Delay(this.batchTimeout, this.cancelScheduleTokenSource.Token)
                    .ContinueWith(_ => this.Dispatch(context, next));
            }

            if (this.batch.Count == this.batch.Capacity)
            {
                this.cancelScheduleTokenSource.Cancel();
                await (await this.dispatchTask.ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        private async Task Dispatch(IMessageContext context, MiddlewareDelegate next)
        {
            var localBatch = Interlocked.Exchange(ref this.batch, new(this.batchSize));

            try
            {
                var batchContext = new BatchConsumeMessageContext(
                    context.ClusterName,
                    context.ConsumerContext,
                    localBatch);

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
                foreach (var messageContext in localBatch)
                {
                    messageContext.ConsumerContext.StoreOffset();
                }
            }
        }
    }
}
