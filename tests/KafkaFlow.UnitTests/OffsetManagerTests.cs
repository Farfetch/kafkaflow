using System;
using System.Collections.Generic;
using Confluent.Kafka;
using FluentAssertions;
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

    private IConsumerContext MockConsumerContext(int offset)
    {
        var mock = new Mock<IConsumerContext>();
        var tpo = new TopicPartitionOffset(_topicPartition.Topic, _topicPartition.Partition, offset);

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
