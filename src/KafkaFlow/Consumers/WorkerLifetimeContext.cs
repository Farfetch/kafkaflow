namespace KafkaFlow.Consumers
{
    internal class WorkerLifetimeContext : IWorkerLifetimeContext
    {
        public IWorker Worker { get; set; }
    }
}
