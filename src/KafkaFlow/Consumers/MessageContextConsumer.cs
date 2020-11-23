namespace KafkaFlow.Consumers
{
    using System;
    using System.Threading;
    using Confluent.Kafka;

    internal class MessageContextConsumer : IMessageContextConsumer
    {
        private readonly IConsumer<byte[], byte[]> consumer;
        private readonly IOffsetManager offsetManager;
        private readonly ConsumeResult<byte[], byte[]> kafkaResult;

        public MessageContextConsumer(
            IConsumer<byte[], byte[]> consumer,
            string name,
            IOffsetManager offsetManager,
            ConsumeResult<byte[], byte[]> kafkaResult,
            CancellationToken workerStopped)
        {
            this.Name = name;
            this.WorkerStopped = workerStopped;
            this.consumer = consumer;
            this.offsetManager = offsetManager;
            this.kafkaResult = kafkaResult;
        }

        public string Name { get; }

        public CancellationToken WorkerStopped { get; }

        public bool ShouldStoreOffset { get; set; } = true;

        public DateTime MessageTimestamp => this.kafkaResult.Message.Timestamp.UtcDateTime;

        public void StoreOffset()
        {
            this.offsetManager.StoreOffset(this.kafkaResult.TopicPartitionOffset);
        }

        public IOffsetsWatermark GetOffsetsWatermark()
        {
            return new OffsetsWatermark(this.consumer.GetWatermarkOffsets(this.kafkaResult.TopicPartition));
        }

        public void Pause()
        {
            this.consumer.Pause(this.consumer.Assignment);
        }

        public void Resume()
        {
            this.consumer.Resume(this.consumer.Assignment);
        }
    }
}
