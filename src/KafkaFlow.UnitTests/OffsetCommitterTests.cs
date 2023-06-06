namespace KafkaFlow.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Confluent.Kafka;
    using FluentAssertions;
    using KafkaFlow.Consumers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class OffsetCommitterTests
    {
        private const int TestTimeout = 5000;

        private Mock<IConsumer> consumerMock;
        private Mock<IDependencyResolver> dependencyResolverMock;

        private Mock<ILogHandler> logHandlerMock;

        private TopicPartition topicPartition;

        private OffsetCommitter offsetCommitter;

        [TestInitialize]
        public void Setup()
        {
            this.consumerMock = new Mock<IConsumer>();
            this.logHandlerMock = new Mock<ILogHandler>();
            this.dependencyResolverMock = new Mock<IDependencyResolver>();
            this.topicPartition = new TopicPartition("topic-A", new Partition(1));

            this.consumerMock
                .Setup(c => c.Configuration.AutoCommitInterval)
                .Returns(TimeSpan.FromMilliseconds(1000));

            this.offsetCommitter = new OffsetCommitter(
                this.consumerMock.Object,
                this.dependencyResolverMock.Object,
                new List<(Action<IDependencyResolver, IEnumerable<TopicPartitionOffset>>, TimeSpan)>(),
                this.logHandlerMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.offsetCommitter.Dispose();
        }

        [TestMethod]
        public void MarkAsProcessed_ShouldCommit()
        {
            // Arrange
            var offset = new TopicPartitionOffset(this.topicPartition, new Offset(1));
            var expectedOffsets = new[] { offset };

            var ready = new ManualResetEvent(false);

            this.consumerMock
                .Setup(c => c.Commit(It.Is<IEnumerable<TopicPartitionOffset>>(l => l.SequenceEqual(expectedOffsets))))
                .Callback((IEnumerable<TopicPartitionOffset> _) => { ready.Set(); });

            // Act
            this.offsetCommitter.MarkAsProcessed(offset);
            ready.WaitOne(TestTimeout);

            // Assert
            this.consumerMock.Verify(
                c => c.Commit(It.Is<IEnumerable<TopicPartitionOffset>>(l => l.SequenceEqual(expectedOffsets))),
                Times.Once);
        }

        [TestMethod]
        public void PendingOffsetsState_ShouldExecuteHandlers()
        {
            // Arrange
            var ready = new ManualResetEvent(false);

            using var committer = new OffsetCommitter(
                this.consumerMock.Object,
                this.dependencyResolverMock.Object,
                new List<(Action<IDependencyResolver, IEnumerable<TopicPartitionOffset>> handler, TimeSpan interval)>
                {
                    ((_, _) => ready.Set(), TimeSpan.FromMilliseconds(100)),
                },
                this.logHandlerMock.Object);

            // Act
            committer.MarkAsProcessed(new TopicPartitionOffset(this.topicPartition, new Offset(1)));

            // Assert
            ready.WaitOne(TestTimeout).Should().BeTrue();
        }

        [TestMethod]
        public void MarkAsProcessed_WithFailure_ShouldRequeueFailedOffsetAndCommit()
        {
            // Arrange
            var offset = new TopicPartitionOffset(this.topicPartition, new Offset(2));
            var expectedOffsets = new[] { offset };

            var ready = new ManualResetEvent(false);
            var hasThrown = false;

            this.consumerMock
                .Setup(c => c.Commit(It.Is<IEnumerable<TopicPartitionOffset>>(l => l.SequenceEqual(expectedOffsets))))
                .Callback(
                    (IEnumerable<TopicPartitionOffset> _) =>
                    {
                        if (!hasThrown)
                        {
                            hasThrown = true;
                            throw new InvalidOperationException();
                        }

                        ready.Set();
                    });

            // Act
            this.offsetCommitter.MarkAsProcessed(offset);
            ready.WaitOne(TestTimeout);

            // Assert
            this.consumerMock.Verify(
                c => c.Commit(It.Is<IEnumerable<TopicPartitionOffset>>(l => l.SequenceEqual(expectedOffsets))),
                Times.Exactly(2));
        }
    }
}
