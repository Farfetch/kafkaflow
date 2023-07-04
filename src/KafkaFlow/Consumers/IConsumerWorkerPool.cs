namespace KafkaFlow.Consumers
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;

    internal interface IConsumerWorkerPool
    {
        int CurrentWorkersCount { get; }

        Task StartAsync(IReadOnlyCollection<TopicPartition> partitions);

        Task StopAsync();

        Task EnqueueAsync(
            ConsumeResult<byte[], byte[]> message,
            CancellationToken stopCancellationToken);
    }
}
