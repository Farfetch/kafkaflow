namespace KafkaFlow.Consumers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using TopicPartitionOffset = KafkaFlow.TopicPartitionOffset;

    internal class ConsumerContext : IConsumerContext
    {
        private readonly TaskCompletionSource<TopicPartitionOffset> completionSource = new();
        private readonly IConsumer consumer;
        private readonly IOffsetManager offsetManager;
        private readonly IConsumerWorker worker;
        private readonly IDependencyResolverScope messageDependencyScope;
        private readonly GlobalEvents globalEvents;

        public ConsumerContext(
            IConsumer consumer,
            IOffsetManager offsetManager,
            ConsumeResult<byte[], byte[]> kafkaResult,
            IConsumerWorker worker,
            IDependencyResolverScope messageDependencyScope,
            IDependencyResolver consumerDependencyResolver,
            GlobalEvents globalEvents)
        {
            this.ConsumerDependencyResolver = consumerDependencyResolver;
            this.consumer = consumer;
            this.offsetManager = offsetManager;
            this.worker = worker;
            this.messageDependencyScope = messageDependencyScope;
            this.AutoMessageCompletion = this.consumer.Configuration.AutoMessageCompletion;
            this.TopicPartitionOffset = new TopicPartitionOffset(
                kafkaResult.Topic,
                kafkaResult.Partition.Value,
                kafkaResult.Offset.Value);
            this.MessageTimestamp = kafkaResult.Message.Timestamp.UtcDateTime;
            this.globalEvents = globalEvents;
        }

        public string ConsumerName => this.consumer.Configuration.ConsumerName;

        public CancellationToken WorkerStopped => this.worker.StopCancellationToken;

        public int WorkerId => this.worker.Id;

        public IDependencyResolver WorkerDependencyResolver => this.worker.WorkerDependencyResolver;

        public IDependencyResolver ConsumerDependencyResolver { get; }

        public string Topic => this.TopicPartitionOffset.Topic;

        public int Partition => this.TopicPartitionOffset.Partition;

        public long Offset => this.TopicPartitionOffset.Offset;

        public TopicPartitionOffset TopicPartitionOffset { get; }

        public string GroupId => this.consumer.Configuration.GroupId;

        public bool AutoMessageCompletion { get; set; }

        public bool ShouldStoreOffset { get; set; } = true;

        public DateTime MessageTimestamp { get; }

        public Task<TopicPartitionOffset> Completion => this.completionSource.Task;

        public void Complete(IMessageContext context)
        {
            this.globalEvents.FireMessageConsumeCompletedAsync(
                       new MessageEventContext(context));

            if (this.ShouldStoreOffset)
            {
                this.offsetManager.MarkAsProcessed(this);
            }

            this.messageDependencyScope.Dispose();
            this.completionSource.TrySetResult(this.TopicPartitionOffset);
        }

        public IOffsetsWatermark GetOffsetsWatermark() =>
            new OffsetsWatermark(
                this.consumer.GetWatermarkOffsets(
                    new TopicPartition(
                        this.TopicPartitionOffset.Topic,
                        this.TopicPartitionOffset.Partition)));

        public void Pause() => this.consumer.FlowManager.Pause(this.consumer.Assignment);

        public void Resume() => this.consumer.FlowManager.Resume(this.consumer.Assignment);
    }
}
