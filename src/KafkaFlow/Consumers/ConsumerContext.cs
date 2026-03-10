using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using KafkaFlow.Extensions;

namespace KafkaFlow.Consumers;

internal class ConsumerContext : IConsumerContext
{
    private readonly TaskCompletionSource<TopicPartitionOffset> _completionSource = new();
    private readonly IConsumer _consumer;
    private readonly IOffsetManager _offsetManager;
    private readonly IConsumerWorker _worker;
    private readonly IDependencyResolverScope _messageDependencyScope;

    public ConsumerContext(
        IConsumer consumer,
        IOffsetManager offsetManager,
        ConsumeResult<byte[], byte[]> kafkaResult,
        IConsumerWorker worker,
        IDependencyResolverScope messageDependencyScope,
        IDependencyResolver consumerDependencyResolver)
    {
        this.ConsumerDependencyResolver = consumerDependencyResolver;
        _consumer = consumer;
        _offsetManager = offsetManager;
        _worker = worker;
        _messageDependencyScope = messageDependencyScope;
        this.AutoMessageCompletion = _consumer.Configuration.AutoMessageCompletion;
        this.TopicPartitionOffset = new TopicPartitionOffset(
            kafkaResult.Topic,
            kafkaResult.Partition.Value,
            kafkaResult.Offset.Value);
        this.MessageTimestamp = kafkaResult.Message.Timestamp.UtcDateTime;
    }

    public string ConsumerName => _consumer.Configuration.ConsumerName;

    public CancellationToken WorkerStopped => _worker.StopCancellationToken;

    public int WorkerId => _worker.Id;

    public IDependencyResolver WorkerDependencyResolver => _worker.WorkerDependencyResolver;

    public IDependencyResolver ConsumerDependencyResolver { get; }

    public string Topic => this.TopicPartitionOffset.Topic;

    public int Partition => this.TopicPartitionOffset.Partition;

    public long Offset => this.TopicPartitionOffset.Offset;

    public TopicPartitionOffset TopicPartitionOffset { get; }

    public string GroupId => _consumer.Configuration.GroupId;

    public bool AutoMessageCompletion { get; set; }

    public bool ShouldStoreOffset { get; set; } = true;

    public DateTime MessageTimestamp { get; }

    public Task<TopicPartitionOffset> Completion => _completionSource.Task;

    public void Complete()
    {
        if (this.ShouldStoreOffset)
        {
            _offsetManager.MarkAsProcessed(this);
        }

        _messageDependencyScope.Dispose();
        _completionSource.TrySetResult(this.TopicPartitionOffset);
    }

    public IOffsetsWatermark GetOffsetsWatermark() =>
        new OffsetsWatermark(
            _consumer.GetWatermarkOffsets(
                new Confluent.Kafka.TopicPartition(
                    this.TopicPartitionOffset.Topic,
                    this.TopicPartitionOffset.Partition)));

    public void Pause() => _consumer.FlowManager.Pause(_consumer.Assignment);

    public void Resume() => _consumer.FlowManager.Resume(_consumer.Assignment);

    public void Pause(IReadOnlyList<TopicPartition> topicPartitions)
    {
        var affectedPartitions = topicPartitions.Select(x => new Confluent.Kafka.TopicPartition(x.Topic, x.Partition));
        _consumer.FlowManager.Pause(_consumer.Assignment.Intersect(affectedPartitions).ToList());
    }

    public void Resume(IReadOnlyList<TopicPartition> topicPartitions)
    {
        var affectedPartitions = topicPartitions.Select(x => new Confluent.Kafka.TopicPartition(x.Topic, x.Partition));
        _consumer.FlowManager.Resume(_consumer.Assignment.Intersect(affectedPartitions).ToList());
    }
}
