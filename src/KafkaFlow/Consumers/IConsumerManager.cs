namespace KafkaFlow.Consumers
{
    using System.Threading.Tasks;

    internal interface IConsumerManager
    {
        IWorkerPoolFeeder Feeder { get; }

        IConsumerWorkerPool WorkerPool { get; }

        IConsumer Consumer { get; }

        Task StartAsync();

        Task StopAsync();
    }
}
