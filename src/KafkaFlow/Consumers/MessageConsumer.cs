namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;

    internal class MessageConsumer : IMessageConsumer
    {
        public MessageConsumer(
            IConsumer<byte[], byte[]> consumer,
            string consumerName,
            string groupId)
        {
            this.Consumer = consumer;
            this.ConsumerName = consumerName;
            this.GroupId = groupId;
        }

        public IConsumer<byte[], byte[]> Consumer { get; }

        public string ConsumerName { get; }

        public string GroupId { get; }

        public IReadOnlyList<string> Subscription => throw new NotImplementedException();

        public IReadOnlyList<TopicPartition> Assignment => throw new NotImplementedException();

        public string MemberId => throw new NotImplementedException();

        public string ClientInstanceName => throw new NotImplementedException();

        public void Pause(IEnumerable<TopicPartition> topicPartitions) =>
            throw new NotImplementedException();

        public void Resume(IEnumerable<TopicPartition> topicPartitions) =>
            throw new NotImplementedException();

        public Offset GetPosition(TopicPartition topicPartition) =>
            throw new NotImplementedException();

        public WatermarkOffsets GetWatermarkOffsets(TopicPartition topicPartition) =>
            throw new NotImplementedException();

        public WatermarkOffsets QueryWatermarkOffsets(TopicPartition topicPartition, TimeSpan timeout) =>
            throw new NotImplementedException();

        public List<TopicPartitionOffset> OffsetsForTimes(
            IEnumerable<TopicPartitionTimestamp> topicPartitions,
            TimeSpan timeout) =>
            throw new NotImplementedException();
    }
}
