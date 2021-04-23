namespace KafkaFlow.UnitTests
{
    using System.Collections.Generic;
    using Confluent.Kafka;
    using KafkaFlow.Consumers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    internal class OffsetManagerTests
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
        public void StoreOffset_WithInvalidTopicPartition_ShouldDoNothing()
        {
            // Arrange
            this.target.AddOffset(new TopicPartitionOffset(this.topicPartition, new Offset(1)));

            // Act
            this.target.StoreOffset(new TopicPartitionOffset(new TopicPartition("topic-B", new Partition(1)), new Offset(1)));

            // Assert
            this.committerMock.Verify(c => c.StoreOffset(It.IsAny<TopicPartitionOffset>()), Times.Never());
        }

        [TestMethod]
        public void StoreOffset_WithGaps_ShouldStoreOffsetJustOnce()
        {
            // Arrange
            this.target.AddOffset(new TopicPartitionOffset(this.topicPartition, new Offset(1)));
            this.target.AddOffset(new TopicPartitionOffset(this.topicPartition, new Offset(2)));
            this.target.AddOffset(new TopicPartitionOffset(this.topicPartition, new Offset(3)));

            // Act
            this.target.StoreOffset(new TopicPartitionOffset(this.topicPartition, new Offset(3)));
            this.target.StoreOffset(new TopicPartitionOffset(this.topicPartition, new Offset(2)));
            this.target.StoreOffset(new TopicPartitionOffset(this.topicPartition, new Offset(1)));

            // Assert
            this.committerMock.Verify(
                c =>
                    c.StoreOffset(
                        It.Is<TopicPartitionOffset>(
                            p =>
                                p.Partition.Equals(this.topicPartition.Partition) &&
                                p.Offset.Value.Equals(4))),
                Times.Once);
        }
    }
}
