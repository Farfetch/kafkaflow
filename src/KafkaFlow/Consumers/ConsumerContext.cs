namespace KafkaFlow.Consumers
{
    using System;
    using System.Threading;
    using Confluent.Kafka;

    internal class ConsumerContext : IConsumerContext
    {
        private readonly IConsumer consumer;
        private readonly IOffsetManager offsetManager;
        private readonly ConsumeResult<byte[], byte[]> kafkaResult;

        public ConsumerContext(
            IConsumer consumer,
            IOffsetManager offsetManager,
            ConsumeResult<byte[], byte[]> kafkaResult,
            CancellationToken workerStopped,
            int workerId)
        {
            this.WorkerStopped = workerStopped;
            this.WorkerId = workerId;
            this.consumer = consumer;
            this.offsetManager = offsetManager;
            this.kafkaResult = kafkaResult;
        }

        public string ConsumerName => this.consumer.Configuration.ConsumerName;

        public CancellationToken WorkerStopped { get; }

        public int WorkerId { get; }

        public string Topic => this.kafkaResult.Topic;

        public int Partition => this.kafkaResult.Partition.Value;

        public long Offset => this.kafkaResult.Offset.Value;

        public string GroupId => this.consumer.Configuration.GroupId;

        public bool ShouldStoreOffset { get; set; } = true;

        public DateTime MessageTimestamp => this.kafkaResult.Message.Timestamp.UtcDateTime;

        public void StoreOffset() => this.offsetManager.StoreOffset(this.kafkaResult.TopicPartitionOffset);

        public IOffsetsWatermark GetOffsetsWatermark() =>
            new OffsetsWatermark(this.consumer.GetWatermarkOffsets(this.kafkaResult.TopicPartition));

        public void Pause() => this.consumer.FlowManager.Pause(this.consumer.Assignment);

        public void Resume() => this.consumer.FlowManager.Resume(this.consumer.Assignment);
    }
}
