namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using KafkaFlow.Configuration;

    internal interface IKafkaConsumer
    {
        ConsumerConfiguration Configuration { get; }

        IReadOnlyList<string> Subscription { get; }

        IReadOnlyList<TopicPartition> Assignment { get; }

        IKafkaConsumerFlowManager FlowManager { get; }

        string MemberId { get; }

        string ClientInstanceName { get; }

        Task StartAsync();

        Task StopAsync();

        Offset GetPosition(TopicPartition topicPartition);

        WatermarkOffsets GetWatermarkOffsets(TopicPartition topicPartition);

        WatermarkOffsets QueryWatermarkOffsets(TopicPartition topicPartition, TimeSpan timeout);

        List<TopicPartitionOffset> OffsetsForTimes(
            IEnumerable<TopicPartitionTimestamp> topicPartitions,
            TimeSpan timeout);

        void Commit(IEnumerable<TopicPartitionOffset> offsetsValues);
    }
}
