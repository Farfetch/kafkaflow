namespace KafkaFlow.UnitTests
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;
    using FluentAssertions;
    using KafkaFlow.Consumers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using TopicPartitionOffset = KafkaFlow.TopicPartitionOffset;

    [TestClass]
    public class OffsetManagerTests
    {
        private Mock<IOffsetCommitter> committerMock;
        private OffsetManager target;
        private TopicPartition topicPartition;

        [TestInitialize]
        public void Setup()
        {
            this.committerMock = new Mock<IOffsetCommitter>();
            this.topicPartition = new TopicPartition("topic-A", new Partition(1));

            this.target = new OffsetManager(
                this.committerMock.Object,
                new List<TopicPartition> { this.topicPartition });
        }

        [TestMethod]
        public void MarkAsProcessed_WithNotQueuedContext_ShouldThrowInvalidOperation()
        {
            // Act
            Action act = () => this.target.MarkAsProcessed(this.MockConsumerContext(1));

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void MarkAsProcessed_WithGaps_ShouldStoreOffsetJustOnce()
        {
            // Arrange
            this.target.Enqueue(this.MockConsumerContext(1));
            this.target.Enqueue(this.MockConsumerContext(2));
            this.target.Enqueue(this.MockConsumerContext(3));

            // Act
            this.target.MarkAsProcessed(this.MockConsumerContext(3));
            this.target.MarkAsProcessed(this.MockConsumerContext(2));
            this.target.MarkAsProcessed(this.MockConsumerContext(1));

            // Assert
            this.committerMock.Verify(
                c =>
                    c.MarkAsProcessed(
                        It.Is<TopicPartitionOffset>(
                            p =>
                                p.Partition == this.topicPartition.Partition &&
                                p.Offset == 3)),
                Times.Once);
        }

        private IConsumerContext MockConsumerContext(int offset)
        {
            var mock = new Mock<IConsumerContext>();
            var tpo = new TopicPartitionOffset(this.topicPartition.Topic, this.topicPartition.Partition, offset);

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
}
