namespace KafkaFlow.UnitTests
{
    using System.Collections.Generic;
    using Confluent.Kafka;
    using KafkaFlow.Consumers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class OffsetManagerTests
    {
        private Mock<IOffsetCommitter> committerMock;
        private TopicPartition topicPartition;
        private OffsetManager target;

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
        public void MarkAsProcessed_WithInvalidTopicPartition_ShouldDoNothing()
        {
            // Arrange
            this.target.Enqueue(new TopicPartitionOffset(this.topicPartition, new Offset(1)));

            // Act
            this.target.MarkAsProcessed(new TopicPartitionOffset(new TopicPartition("topic-B", new Partition(1)), new Offset(1)));

            // Assert
            this.committerMock.Verify(c => c.MarkAsProcessed(It.IsAny<TopicPartitionOffset>()), Times.Never());
        }

        [TestMethod]
        public void MarkAsProcessed_WithGaps_ShouldStoreOffsetJustOnce()
        {
            // Arrange
            this.target.Enqueue(new TopicPartitionOffset(this.topicPartition, new Offset(1)));
            this.target.Enqueue(new TopicPartitionOffset(this.topicPartition, new Offset(2)));
            this.target.Enqueue(new TopicPartitionOffset(this.topicPartition, new Offset(3)));

            // Act
            this.target.MarkAsProcessed(new TopicPartitionOffset(this.topicPartition, new Offset(3)));
            this.target.MarkAsProcessed(new TopicPartitionOffset(this.topicPartition, new Offset(2)));
            this.target.MarkAsProcessed(new TopicPartitionOffset(this.topicPartition, new Offset(1)));

            // Assert
            this.committerMock.Verify(
                c =>
                    c.MarkAsProcessed(
                        It.Is<TopicPartitionOffset>(
                            p =>
                                p.Partition.Equals(this.topicPartition.Partition) &&
                                p.Offset.Value.Equals(4))),
                Times.Once);
        }
    }
}
