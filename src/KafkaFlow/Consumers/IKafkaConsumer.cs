namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Confluent.Kafka;

    internal interface IKafkaConsumer
    {
        IReadOnlyList<string> Subscription { get; }

        IReadOnlyList<TopicPartition> Assignment { get; }

        string MemberId { get; }

        string ClientInstanceName { get; }

        Task StartAsync();

        Task StopAsync();

        void Pause(IEnumerable<TopicPartition> topicPartitions);

        void Resume(IEnumerable<TopicPartition> topicPartitions);

        Offset GetPosition(TopicPartition topicPartition);

        WatermarkOffsets GetWatermarkOffsets(TopicPartition topicPartition);

        WatermarkOffsets QueryWatermarkOffsets(TopicPartition topicPartition, TimeSpan timeout);

        List<TopicPartitionOffset> OffsetsForTimes(
            IEnumerable<TopicPartitionTimestamp> topicPartitions,
            TimeSpan timeout);

        void Commit(IEnumerable<TopicPartitionOffset> offsetsValues);
    }
}
