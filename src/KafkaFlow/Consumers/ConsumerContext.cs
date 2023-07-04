namespace KafkaFlow.Consumers
{
    using System;
    using System.Threading;
    using Confluent.Kafka;

    internal class ConsumerContext : IConsumerContext
    {
        private readonly IConsumer consumer;
        private readonly IOffsetManager offsetManager;
        private readonly TopicPartitionOffset topicPartitionOffset;

        public ConsumerContext(
            IConsumer consumer,
            IOffsetManager offsetManager,
            ConsumeResult<byte[], byte[]> kafkaResult,
            CancellationToken workerStopped,
            int workerId,
            IDependencyResolver workerDependencyResolver,
            IDependencyResolver consumerDependencyResolver)
        {
            this.WorkerStopped = workerStopped;
            this.WorkerId = workerId;
            this.WorkerDependencyResolver = workerDependencyResolver;
            this.ConsumerDependencyResolver = consumerDependencyResolver;
            this.consumer = consumer;
            this.offsetManager = offsetManager;
            this.topicPartitionOffset = kafkaResult.TopicPartitionOffset;
            this.MessageTimestamp = kafkaResult.Message.Timestamp.UtcDateTime;
        }

        public string ConsumerName => this.consumer.Configuration.ConsumerName;

        public CancellationToken WorkerStopped { get; }

        public int WorkerId { get; }

        public IDependencyResolver WorkerDependencyResolver { get; }

        public IDependencyResolver ConsumerDependencyResolver { get; }

        public string Topic => this.topicPartitionOffset.Topic;

        public int Partition => this.topicPartitionOffset.Partition.Value;

        public long Offset => this.topicPartitionOffset.Offset.Value;

        public string GroupId => this.consumer.Configuration.GroupId;

        public bool ShouldStoreOffset { get; set; } = true;

        public DateTime MessageTimestamp { get; }

        public void StoreOffset() => this.offsetManager.MarkAsProcessed(this.topicPartitionOffset);

        public IOffsetsWatermark GetOffsetsWatermark() =>
            new OffsetsWatermark(this.consumer.GetWatermarkOffsets(this.topicPartitionOffset.TopicPartition));

        public void Pause() => this.consumer.FlowManager.Pause(this.consumer.Assignment);

        public void Resume() => this.consumer.FlowManager.Resume(this.consumer.Assignment);
    }
}
