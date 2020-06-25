namespace KafkaFlow.Consumers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;

    internal interface IConsumerWorker : IWorker
    {
        ValueTask EnqueueAsync(ConsumeResult<byte[], byte[]> message, CancellationToken stopCancellationToken = default);

        Task StartAsync(CancellationToken stopCancellationToken = default);

        Task StopAsync();
    }
}
