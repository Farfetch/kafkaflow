namespace KafkaFlow.Consumers
{
    internal class WorkerContext : IWorkerContext
    {
        public IWorker Worker { get; set; }
    }
}
