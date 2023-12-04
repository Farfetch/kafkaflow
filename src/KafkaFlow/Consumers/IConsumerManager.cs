using System.Threading.Tasks;

namespace KafkaFlow.Consumers;

internal interface IConsumerManager
{
    IWorkerPoolFeeder Feeder { get; }

    IConsumerWorkerPool WorkerPool { get; }

    IConsumer Consumer { get; }

    Task StartAsync();

    Task StopAsync();
}
