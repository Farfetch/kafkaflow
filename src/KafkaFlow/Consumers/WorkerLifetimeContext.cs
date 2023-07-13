namespace KafkaFlow.Consumers
{
    internal class WorkerLifetimeContext : IWorkerLifetimeContext
    {
        public IWorker Worker { get; set; }

        public IConsumer Consumer { get; set; }
    }
}
