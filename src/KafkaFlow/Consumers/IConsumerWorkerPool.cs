namespace KafkaFlow.Consumers
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;

    internal interface IConsumerWorkerPool
    {
        Task StartAsync(
            IConsumer<byte[], byte[]> consumer,
            IEnumerable<TopicPartition> partitions,
            CancellationToken stopCancellationToken);

        Task StopAsync();

        Task EnqueueAsync(
            ConsumeResult<byte[], byte[]> message,
            CancellationToken stopCancellationToken);
    }
}
