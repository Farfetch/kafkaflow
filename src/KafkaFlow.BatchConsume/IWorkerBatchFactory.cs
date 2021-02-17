namespace KafkaFlow.BatchConsume
{
    using System;

    internal interface IWorkerBatchFactory
    {
        IWorkerBatch Create(int batchSize, TimeSpan batchTimeout, ILogHandler logHandler);
    }
}
