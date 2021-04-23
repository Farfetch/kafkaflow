namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using KafkaFlow.Configuration;

    internal interface IConsumer : IDisposable
    {
        IConsumerConfiguration Configuration { get; }

        IReadOnlyList<string> Subscription { get; }

        IReadOnlyList<TopicPartition> Assignment { get; }

        IConsumerFlowManager FlowManager { get; }

        string MemberId { get; }

        string ClientInstanceName { get; }

        void OnPartitionsAssigned(Action<IConsumer<byte[], byte[]>, List<TopicPartition>> handler);

        void OnPartitionsRevoked(Action<IConsumer<byte[], byte[]>, List<TopicPartitionOffset>> handler);

        void OnError(Action<IConsumer<byte[], byte[]>, Error> handler);

        void OnStatistics(Action<IConsumer<byte[], byte[]>, string> handler);

        Offset GetPosition(TopicPartition topicPartition);

        WatermarkOffsets GetWatermarkOffsets(TopicPartition topicPartition);

        WatermarkOffsets QueryWatermarkOffsets(TopicPartition topicPartition, TimeSpan timeout);

        List<TopicPartitionOffset> OffsetsForTimes(
            IEnumerable<TopicPartitionTimestamp> topicPartitions,
            TimeSpan timeout);

        void Commit(IEnumerable<TopicPartitionOffset> offsetsValues);

        ValueTask<ConsumeResult<byte[], byte[]>> ConsumeAsync(CancellationToken cancellationToken);
    }
}
