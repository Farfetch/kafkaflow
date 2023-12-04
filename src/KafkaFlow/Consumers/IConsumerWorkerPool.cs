using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace KafkaFlow.Consumers;

internal interface IConsumerWorkerPool
{
    int CurrentWorkersCount { get; }

    Task StartAsync(IReadOnlyCollection<TopicPartition> partitions, int workersCount);

    Task StopAsync();

    Task EnqueueAsync(
        ConsumeResult<byte[], byte[]> message,
        CancellationToken stopCancellationToken);
}
