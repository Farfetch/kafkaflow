using System;
using System.Collections.Generic;
using Confluent.Kafka;
using AwesomeAssertions;
using KafkaFlow.Consumers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KafkaFlow.UnitTests;

[TestClass]
public class OffsetManagerTests
{
    private Mock<IOffsetCommitter> _committerMock;
    private OffsetManager _target;
    private Confluent.Kafka.TopicPartition _topicPartition;

    [TestInitialize]
    public void Setup()
    {
        _committerMock = new Mock<IOffsetCommitter>();
        _topicPartition = new Confluent.Kafka.TopicPartition("topic-A", new Partition(1));

        _target = new OffsetManager(
            _committerMock.Object,
            new List<Confluent.Kafka.TopicPartition> { _topicPartition });
    }

    [TestMethod]
    public void MarkAsProcessed_WithNotQueuedContext_ShouldThrowInvalidOperation()
    {
        // Act
        Action act = () => _target.MarkAsProcessed(this.MockConsumerContext(1));

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    public void MarkAsProcessed_WithGaps_ShouldStoreOffsetJustOnce()
    {
        // Arrange
        _target.Enqueue(this.MockConsumerContext(1));
        _target.Enqueue(this.MockConsumerContext(2));
        _target.Enqueue(this.MockConsumerContext(3));

        // Act
        _target.MarkAsProcessed(this.MockConsumerContext(3));
        _target.MarkAsProcessed(this.MockConsumerContext(2));
        _target.MarkAsProcessed(this.MockConsumerContext(1));

        // Assert
        _committerMock.Verify(
            c =>
                c.MarkAsProcessed(
                    It.Is<TopicPartitionOffset>(
                        p =>
                            p.Partition == _topicPartition.Partition &&
                            p.Offset == 3)),
            Times.Once);
    }

    [TestMethod]
    public void Enqueue_ForRevokedPartition_SilentlyDropsMessage_CausingOffsetLoss()
    {
        // This test proves the mechanism behind Bug #1 in ConsumerManager:
        // When the feeder is not stopped during rebalance, it can consume a message
        // from a partition that was just revoked. That message gets enqueued into a
        // new OffsetManager (created for the new partition set). Since the revoked
        // partition is not in the new OffsetManager, Enqueue and MarkAsProcessed
        // both silently no-op. The message is processed by a worker but its offset
        // is NEVER committed. After the rebalance the broker replays that offset.
        var revokedPartition = new Confluent.Kafka.TopicPartition("topic-A", new Partition(99));
        var context = MockConsumerContext(100, revokedPartition.Topic, revokedPartition.Partition.Value);

        // Act - message from revoked partition arrives in new OffsetManager
        _target.Enqueue(context);
        _target.MarkAsProcessed(context);

        // Assert - offset is silently dropped, committer is never called
        _committerMock.Verify(
            c => c.MarkAsProcessed(It.IsAny<TopicPartitionOffset>()),
            Times.Never,
            "Offset from a partition unknown to this OffsetManager is silently discarded");
    }

    private IConsumerContext MockConsumerContext(int offset) =>
        MockConsumerContext(offset, _topicPartition.Topic, _topicPartition.Partition.Value);

    private IConsumerContext MockConsumerContext(int offset, string topic, int partition)
    {
        var mock = new Mock<IConsumerContext>();
        var tpo = new TopicPartitionOffset(topic, partition, offset);

        mock
            .SetupGet(x => x.Offset)
            .Returns(tpo.Offset);

        mock
            .SetupGet(x => x.Partition)
            .Returns(tpo.Partition);

        mock
            .SetupGet(x => x.Topic)
            .Returns(tpo.Topic);

        mock
            .SetupGet(x => x.TopicPartitionOffset)
            .Returns(tpo);

        return mock.Object;
    }
}
