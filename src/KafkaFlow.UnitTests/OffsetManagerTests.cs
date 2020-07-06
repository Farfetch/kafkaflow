namespace KafkaFlow.UnitTests
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;
    using Consumers;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class OffsetManagerTests
    {
        private Mock<IConsumer<byte[], byte[]>> consumerMock;
        private TopicPartition topicPartition;
        private OffsetManager target;

        [TestInitialize]
        public void Setup()
        {
            this.consumerMock = new Mock<IConsumer<byte[], byte[]>>();
            this.topicPartition = new TopicPartition("topic-A", new Partition(1));
            this.target = new OffsetManager(
                this.consumerMock.Object, 
                new List<TopicPartition> { this.topicPartition });
        }
        
        [TestMethod]
        public void StoreOffset_WithoutInitialization_ThrowsException()
        {
            // Act
            Action act = () => this.target.StoreOffset(new TopicPartitionOffset(this.topicPartition, new Offset(1) ));
            
            // Assert
            act.Should().Throw<InvalidOperationException>();
        }
        
        [TestMethod]
        public void StoreOffset_WithInvalidTopicPartition_ShouldDoNothing()
        {
            // Arrange
            this.target.InitializeOffsetIfNeeded(new TopicPartitionOffset(this.topicPartition, new Offset(1)));
            
            // Act
            this.target.StoreOffset(new TopicPartitionOffset(new TopicPartition("topic-B", new Partition(1)), new Offset(1)));
            
            // Assert
            this.consumerMock.Verify(c => c.StoreOffset(It.IsAny<TopicPartitionOffset>()), Times.Never());
        }
        
        [TestMethod]
        public void StoreOffset_WithGaps_ShouldStoreOffsetJustOnce()
        {
            // Arrange
            this.target.InitializeOffsetIfNeeded(new TopicPartitionOffset(this.topicPartition, new Offset(1)));
            
            // Act
            this.target.StoreOffset(new TopicPartitionOffset(this.topicPartition, new Offset(3)));
            this.target.StoreOffset(new TopicPartitionOffset(this.topicPartition, new Offset(2)));
            this.target.StoreOffset(new TopicPartitionOffset(this.topicPartition, new Offset(1)));
            
            // Assert
            this.consumerMock.Verify(c => 
                c.StoreOffset(It.Is<TopicPartitionOffset>(p => 
                    p.Partition.Equals(this.topicPartition.Partition) &&
                    p.Offset.Value.Equals(4))), 
                Times.Once);
        }
    }
}