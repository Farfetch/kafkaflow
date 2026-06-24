using System.Threading.Tasks;

namespace KafkaFlow.Consumers;

internal interface IWorkerPoolFeeder
{
    void Start();

    // Cancels the feeder token without awaiting the feeder task.
    // Safe to call from Confluent.Kafka rebalance callbacks (which run on the polling
    // thread), where awaiting the feeder task would deadlock.
    void Cancel();

    Task StopAsync();
}
