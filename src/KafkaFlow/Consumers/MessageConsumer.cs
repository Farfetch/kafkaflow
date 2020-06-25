namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;

    internal class MessageConsumer : IMessageConsumer
    {
        private readonly IConsumer<byte[], byte[]> consumer;

        public MessageConsumer(
            IConsumer<byte[], byte[]> consumer,
            string consumerName,
            string groupId)
        {
            this.ConsumerName = consumerName;
            this.GroupId = groupId;
            this.consumer = consumer;
        }

        public string ConsumerName { get; }
        
        public string GroupId { get; }

        public IReadOnlyList<string> Subscription => this.consumer.Subscription.AsReadOnly();

        public IReadOnlyList<TopicPartition> Assignment => this.consumer.Assignment.AsReadOnly();

        public string MemberId => this.consumer.MemberId;

        public string ClientInstanceName => this.consumer.Name;

        public void Pause(IEnumerable<TopicPartition> topicPartitions) =>
            this.consumer.Pause(topicPartitions);

        public void Resume(IEnumerable<TopicPartition> topicPartitions) =>
            this.consumer.Resume(topicPartitions);

        public Offset GetPosition(TopicPartition topicPartition) =>
            this.consumer.Position(topicPartition);

        public WatermarkOffsets GetWatermarkOffsets(TopicPartition topicPartition) =>
            this.consumer.GetWatermarkOffsets(topicPartition);

        public WatermarkOffsets QueryWatermarkOffsets(TopicPartition topicPartition, TimeSpan timeout) =>
            this.consumer.QueryWatermarkOffsets(topicPartition, timeout);

        public void OffsetsForTimes(IEnumerable<TopicPartitionTimestamp> topicPartitions, TimeSpan timeout) =>
            this.consumer.OffsetsForTimes(topicPartitions, timeout);
    }
}