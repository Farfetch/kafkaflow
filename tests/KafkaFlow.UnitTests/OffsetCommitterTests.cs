using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KafkaFlow.Consumers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KafkaFlow.UnitTests;

[TestClass]
public class OffsetCommitterTests
{
    private const int TestTimeout = 5000;

    private Mock<IConsumer> _consumerMock;

    private Confluent.Kafka.TopicPartition _topicPartition;

    private OffsetCommitter _offsetCommitter;

    [TestInitialize]
    public async Task Setup()
    {
        _consumerMock = new Mock<IConsumer>();
        _topicPartition = new Confluent.Kafka.TopicPartition("topic-A", new Confluent.Kafka.Partition(1));

        _consumerMock
            .Setup(c => c.Configuration.AutoCommitInterval)
            .Returns(TimeSpan.FromMilliseconds(1000));

        _offsetCommitter = new OffsetCommitter(
            _consumerMock.Object,
            Mock.Of<IDependencyResolver>(),
            Mock.Of<ILogHandler>());

        await _offsetCommitter.StartAsync();
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        await _offsetCommitter.StopAsync();
    }

    [TestMethod]
    public async Task MarkAsProcessed_ShouldCommit()
    {
        // Arrange
        var expectedOffsets = new[] { new Confluent.Kafka.TopicPartitionOffset(_topicPartition, new Confluent.Kafka.Offset(2)) };

        var ready = new TaskCompletionSource().WithTimeout(TestTimeout);

        _consumerMock
            .Setup(c => c.Commit(It.Is<IReadOnlyCollection<Confluent.Kafka.TopicPartitionOffset>>(l => l.SequenceEqual(expectedOffsets))))
            .Callback((IEnumerable<Confluent.Kafka.TopicPartitionOffset> _) => ready.SetResult());

        // Act
        _offsetCommitter.MarkAsProcessed(
            new KafkaFlow.TopicPartitionOffset(
                _topicPartition.Topic,
                _topicPartition.Partition,
                1));

        await ready.Task;

        // Assert
        _consumerMock.VerifyAll();
    }

    [TestMethod]
    public async Task PendingOffsetsState_ShouldExecuteHandlers()
    {
        // Arrange
        var ready = new TaskCompletionSource().WithTimeout(TestTimeout);

        var committer = new OffsetCommitter(
            _consumerMock.Object,
            Mock.Of<IDependencyResolver>(),
            Mock.Of<ILogHandler>());

        committer.PendingOffsetsStatisticsHandlers.Add(new((_, _) => ready.TrySetResult(), TimeSpan.FromMilliseconds(100)));

        await committer.StartAsync();

        // Act
        committer.MarkAsProcessed(
            new KafkaFlow.TopicPartitionOffset(
                _topicPartition.Topic,
                _topicPartition.Partition,
                1));

        // Assert
        await ready.Task;

        // Cleanup
        await committer.StopAsync();
    }

    [TestMethod]
    public async Task MarkAsProcessed_WithFailure_ShouldRequeueFailedOffsetAndCommit()
    {
        // Arrange
        var expectedOffsets = new[] { new Confluent.Kafka.TopicPartitionOffset(_topicPartition, new Confluent.Kafka.Offset(2)) };

        var ready = new TaskCompletionSource().WithTimeout(TestTimeout);
        var hasThrown = false;

        _consumerMock
            .Setup(c => c.Commit(It.Is<IReadOnlyCollection<Confluent.Kafka.TopicPartitionOffset>>(l => l.SequenceEqual(expectedOffsets))))
            .Callback(
                (IEnumerable<Confluent.Kafka.TopicPartitionOffset> _) =>
                {
                    if (!hasThrown)
                    {
                        hasThrown = true;
                        throw new InvalidOperationException();
                    }

                    ready.TrySetResult();
                });

        // Act
        _offsetCommitter.MarkAsProcessed(
            new KafkaFlow.TopicPartitionOffset(
                _topicPartition.Topic,
                _topicPartition.Partition,
                1));

        await ready.Task;

        // Assert
        _consumerMock.Verify(
            c => c.Commit(It.Is<IReadOnlyCollection<Confluent.Kafka.TopicPartitionOffset>>(l => l.SequenceEqual(expectedOffsets))),
            Times.Exactly(2));
    }
}
