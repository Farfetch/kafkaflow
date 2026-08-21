using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Confluent.Kafka;
using KafkaFlow.Configuration;
using KafkaFlow.Consumers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KafkaFlow.UnitTests.Consumer;

/// <summary>
/// Covers the offsets bookkeeping when the worker pool is restarted without a partition assignment change,
/// which is what a workers count change (e.g. the consumer lag balancer) does. The committed offset must never
/// move past a message that was not processed, otherwise that message is silently skipped.
/// </summary>
[TestClass]
public class ConsumerWorkerPoolOffsetTrackingTests
{
    private const string TopicName = "topic-A";
    private const int PartitionNumber = 0;

    private readonly List<Confluent.Kafka.TopicPartition> _partitions = new()
    {
        new Confluent.Kafka.TopicPartition(TopicName, new Partition(PartitionNumber)),
    };

    private readonly ConcurrentDictionary<long, IConsumerContext> _processedContexts = new();
    private readonly List<Confluent.Kafka.TopicPartitionOffset> _committedOffsets = new();

    private TestDistributionStrategy _distributionStrategy;
    private ConsumerWorkerPool _target;

    [TestInitialize]
    public void Setup()
    {
        _distributionStrategy = new TestDistributionStrategy();

        var logHandler = Mock.Of<ILogHandler>();

        var configuration = new Mock<IConsumerConfiguration>();
        configuration.SetupGet(x => x.ConsumerName).Returns("consumer-test");
        configuration.SetupGet(x => x.NoStoreOffsets).Returns(false);
        configuration.SetupGet(x => x.ManagementDisabled).Returns(true);
        configuration.SetupGet(x => x.AutoMessageCompletion).Returns(false);
        configuration.SetupGet(x => x.BufferSize).Returns(100);
        configuration.SetupGet(x => x.WorkerStopTimeout).Returns(TimeSpan.FromMilliseconds(200));
        configuration.SetupGet(x => x.AutoCommitInterval).Returns(TimeSpan.FromMinutes(5));
        configuration.SetupGet(x => x.PendingOffsetsStatisticsHandlers).Returns(new List<PendingOffsetsStatisticsHandler>());
        configuration.SetupGet(x => x.DistributionStrategyFactory).Returns(_ => _distributionStrategy);
        configuration
            .SetupGet(x => x.ClusterConfiguration)
            .Returns(new ClusterConfiguration(null, "cluster", new[] { "localhost:9092" }, null, null, null));

        var consumer = new Mock<IConsumer>();
        consumer.SetupGet(x => x.Configuration).Returns(configuration.Object);
        consumer.SetupGet(x => x.MaxPollIntervalExceeded).Returns(new Event(logHandler));
        consumer
            .Setup(x => x.Commit(It.IsAny<IReadOnlyCollection<Confluent.Kafka.TopicPartitionOffset>>()))
            .Callback<IReadOnlyCollection<Confluent.Kafka.TopicPartitionOffset>>(
                offsets =>
                {
                    lock (_committedOffsets)
                    {
                        _committedOffsets.AddRange(offsets);
                    }
                });

        var resolver = new Mock<IDependencyResolver>();
        var scope = new Mock<IDependencyResolverScope>();
        scope.SetupGet(x => x.Resolver).Returns(() => resolver.Object);
        resolver.Setup(x => x.CreateScope()).Returns(() => scope.Object);
        resolver.Setup(x => x.Resolve(typeof(ConsumerMiddlewareContext))).Returns(() => new ConsumerMiddlewareContext());
        resolver.Setup(x => x.Resolve(typeof(GlobalEvents))).Returns(new GlobalEvents(logHandler));

        // The middleware does nothing but expose the contexts, so the test decides which ones are completed.
        var middlewareExecutor = new Mock<IMiddlewareExecutor>();
        middlewareExecutor
            .Setup(x => x.Execute(It.IsAny<IMessageContext>(), It.IsAny<Func<IMessageContext, Task>>()))
            .Returns<IMessageContext, Func<IMessageContext, Task>>(
                (context, _) =>
                {
                    _processedContexts[context.ConsumerContext.Offset] = context.ConsumerContext;
                    return Task.CompletedTask;
                });

        _target = new ConsumerWorkerPool(
            consumer.Object,
            resolver.Object,
            middlewareExecutor.Object,
            configuration.Object,
            logHandler);
    }

    [TestMethod]
    public async Task ChangingWorkersCount_WithMessageNotProcessed_ShouldNotCommitPastIt()
    {
        // Arrange
        await _target.StartAsync(_partitions, 1);

        await this.EnqueueAsync(0);
        await this.EnqueueAsync(1);
        await this.EnqueueAsync(2);

        this.WaitForContexts(0, 1, 2);

        // Offset 1 is left unprocessed, as if its worker had been cancelled by the stop timeout
        _processedContexts[0].Complete();
        _processedContexts[2].Complete();

        // Act - a workers count change restarts the pool keeping the same partition assignment
        await _target.StopAsync(keepOffsetManager: true);
        await _target.StartAsync(_partitions, 2);

        await this.EnqueueAsync(3);
        this.WaitForContexts(3);
        _processedContexts[3].Complete();

        await _target.StopAsync();

        // Assert - offset 1 was never processed, so the commit cannot go past it
        this.CommittedOffsets().Should().OnlyContain(offset => offset <= 1);
    }

    [TestMethod]
    public async Task ChangingWorkersCount_WithMessageNotAssignedToAnyWorker_ShouldNotCommitPastIt()
    {
        // Arrange
        await _target.StartAsync(_partitions, 1);

        await this.EnqueueAsync(0);
        this.WaitForContexts(0);
        _processedContexts[0].Complete();

        // The pool is stopping, so no worker can take offset 1 anymore
        _distributionStrategy.HasNoWorkerAvailable = true;
        await this.EnqueueAsync(1);
        _distributionStrategy.HasNoWorkerAvailable = false;

        // Act
        await _target.StopAsync(keepOffsetManager: true);
        await _target.StartAsync(_partitions, 1);

        await this.EnqueueAsync(2);
        this.WaitForContexts(2);
        _processedContexts[2].Complete();

        await _target.StopAsync();

        // Assert - offset 1 was never delivered to a worker, so the commit cannot go past it
        this.CommittedOffsets().Should().OnlyContain(offset => offset <= 1);
    }

    [TestMethod]
    public async Task ChangingWorkersCount_WithAllMessagesProcessed_ShouldCommitTheLastOffset()
    {
        // Arrange
        await _target.StartAsync(_partitions, 1);

        await this.EnqueueAsync(0);
        this.WaitForContexts(0);
        _processedContexts[0].Complete();

        await _target.StopAsync(keepOffsetManager: true);
        await _target.StartAsync(_partitions, 2);

        // Act
        await this.EnqueueAsync(1);
        this.WaitForContexts(1);
        _processedContexts[1].Complete();

        await _target.StopAsync();

        // Assert - nothing is pending, so the offset of the next message to read is committed
        this.CommittedOffsets().Should().Contain(2);
    }

    private void WaitForContexts(params long[] offsets) =>
        SpinWait
            .SpinUntil(() => offsets.All(_processedContexts.ContainsKey), TimeSpan.FromSeconds(5))
            .Should()
            .BeTrue("the workers should have processed the enqueued messages");

    private IReadOnlyCollection<long> CommittedOffsets()
    {
        lock (_committedOffsets)
        {
            return _committedOffsets.Select(x => x.Offset.Value).ToList();
        }
    }

    private Task EnqueueAsync(long offset)
    {
        return _target.EnqueueAsync(
            new ConsumeResult<byte[], byte[]>
            {
                TopicPartitionOffset = new Confluent.Kafka.TopicPartitionOffset(
                    TopicName,
                    new Partition(PartitionNumber),
                    new Offset(offset)),
                Message = new Message<byte[], byte[]>
                {
                    Key = null,
                    Value = Array.Empty<byte>(),
                },
            },
            CancellationToken.None);
    }

    private sealed class TestDistributionStrategy : IWorkerDistributionStrategy
    {
        private IReadOnlyList<IWorker> _workers;

        public bool HasNoWorkerAvailable { get; set; }

        public void Initialize(IReadOnlyList<IWorker> workers) => _workers = workers;

        public ValueTask<IWorker> GetWorkerAsync(WorkerDistributionContext context) =>
            new(this.HasNoWorkerAvailable ? null : _workers[0]);
    }
}
