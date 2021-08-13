namespace KafkaFlow.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Confluent.Kafka;
    using KafkaFlow.Consumers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class OffsetCommitterTests
    {
        private const int TestTimeout = 1000;

        private Mock<IConsumer> consumerMock;

        private Mock<ILogHandler> logHandlerMock;

        private TopicPartition topicPartition;

        private OffsetCommitter offsetCommitter;

        [TestInitialize]
        public void Setup()
        {
            this.consumerMock = new Mock<IConsumer>();
            this.logHandlerMock = new Mock<ILogHandler>();
            this.topicPartition = new TopicPartition("topic-A", new Partition(1));

            this.consumerMock.Setup(c => c.Configuration.AutoCommitInterval)
                .Returns(TimeSpan.FromMilliseconds(10));

            this.offsetCommitter = new OffsetCommitter(this.consumerMock.Object, this.logHandlerMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.offsetCommitter.Dispose();
        }

        [TestMethod]
        public void StoreOffset_ShouldCommit()
        {
            // Arrange
            var offset = new TopicPartitionOffset(this.topicPartition, new Offset(1));
            var expectedOffsets = new[] { offset };

            var ready = new ManualResetEvent(false);

            this.consumerMock
                .Setup(c => c.Commit(It.Is<IEnumerable<TopicPartitionOffset>>(l => l.SequenceEqual(expectedOffsets))))
                .Callback((IEnumerable<TopicPartitionOffset> _) =>
                {
                    ready.Set();
                });

            // Act
            this.offsetCommitter.StoreOffset(offset);
            ready.WaitOne(TestTimeout);

            // Assert
            this.consumerMock.Verify(
                c => c.Commit(It.Is<IEnumerable<TopicPartitionOffset>>(l => l.SequenceEqual(expectedOffsets))),
                Times.Once);
        }

        [TestMethod]
        public void StoreOffset_WithFailure_ShouldRequeueFailedOffsetAndCommit()
        {
            // Arrange
            var offset = new TopicPartitionOffset(this.topicPartition, new Offset(2));
            var expectedOffsets = new[] { offset };

            var ready = new ManualResetEvent(false);
            var hasThrown = false;

            this.consumerMock
                .Setup(c => c.Commit(It.Is<IEnumerable<TopicPartitionOffset>>(l => l.SequenceEqual(expectedOffsets))))
                .Callback((IEnumerable<TopicPartitionOffset> _) =>
                {
                    if (!hasThrown)
                    {
                        hasThrown = true;
                        throw new InvalidOperationException();
                    }

                    ready.Set();
                });

            // Act
            this.offsetCommitter.StoreOffset(offset);
            ready.WaitOne(TestTimeout);

            // Assert
            this.consumerMock.Verify(
                c => c.Commit(It.Is<IEnumerable<TopicPartitionOffset>>(l => l.SequenceEqual(expectedOffsets))),
                Times.Exactly(2));
        }
    }
}
