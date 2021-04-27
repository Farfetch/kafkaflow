namespace KafkaFlow.BatchConsume
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    internal class BatchConsumeMiddleware : IMessageMiddleware
    {
        private readonly int batchSize;
        private readonly TimeSpan batchTimeout;
        private readonly IWorkerBatchFactory workerBatchFactory;
        private readonly ILogHandler logHandler;

        private readonly ConcurrentDictionary<int, IWorkerBatch> batches = new();

        public BatchConsumeMiddleware(
            int batchSize,
            TimeSpan batchTimeout,
            IWorkerBatchFactory workerBatchFactory,
            ILogHandler logHandler)
        {
            this.batchSize = batchSize;
            this.batchTimeout = batchTimeout;
            this.workerBatchFactory = workerBatchFactory;
            this.logHandler = logHandler;
        }

        public Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            var workerBatch = this.batches.GetOrAdd(
                context.ConsumerContext.WorkerId,
                _ => this.workerBatchFactory.Create(this.batchSize, this.batchTimeout, this.logHandler));

            context.ConsumerContext.ShouldStoreOffset = false;

            return workerBatch.AddAsync(context, next);
        }
    }
}
