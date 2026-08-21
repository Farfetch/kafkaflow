using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace KafkaFlow.Consumers;

internal interface IConsumerWorkerPool
{
    int CurrentWorkersCount { get; }

    Task StartAsync(IReadOnlyCollection<Confluent.Kafka.TopicPartition> partitions, int workersCount);

    /// <summary>
    /// Stops the workers of the pool.
    /// </summary>
    /// <param name="keepOffsetManager">
    /// When true the offset manager and the offset committer are kept alive, so offsets that were consumed
    /// but not processed are still known by the pool after it is started again. It must only be used when the
    /// partition assignment does not change, otherwise the offset manager will track the wrong partitions.
    /// </param>
    Task StopAsync(bool keepOffsetManager = false);

    Task EnqueueAsync(
        ConsumeResult<byte[], byte[]> message,
        CancellationToken stopCancellationToken);
}
