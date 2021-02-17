namespace KafkaFlow.BatchConsume
{
    using System;

    internal class WorkerBatchFactory : IWorkerBatchFactory
    {
        public static readonly IWorkerBatchFactory Default = new WorkerBatchFactory();

        private WorkerBatchFactory()
        {
        }

        public IWorkerBatch Create(int batchSize, TimeSpan batchTimeout, ILogHandler logHandler)
            => new WorkerBatch(batchSize, batchTimeout, logHandler);
    }
}
