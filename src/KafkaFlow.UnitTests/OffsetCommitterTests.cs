namespace KafkaFlow.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using KafkaFlow.Consumers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class OffsetCommitterTests
    {
        private const int TestTimeout = 5000;

        private Mock<IConsumer> consumerMock;

        private TopicPartition topicPartition;

        private OffsetCommitter offsetCommitter;

        [TestInitialize]
        public async Task Setup()
        {
            this.consumerMock = new Mock<IConsumer>();
            this.topicPartition = new TopicPartition("topic-A", new Partition(1));

            this.consumerMock
                .Setup(c => c.Configuration.AutoCommitInterval)
                .Returns(TimeSpan.FromMilliseconds(1000));

            this.offsetCommitter = new OffsetCommitter(
                this.consumerMock.Object,
                Mock.Of<IDependencyResolver>(),
                Mock.Of<ILogHandler>());

            await this.offsetCommitter.StartAsync();
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await this.offsetCommitter.StopAsync();
        }

        [TestMethod]
        public async Task MarkAsProcessed_ShouldCommit()
        {
            // Arrange
            var expectedOffsets = new[] { new TopicPartitionOffset(this.topicPartition, new Offset(2)) };

            var ready = new TaskCompletionSource().WithTimeout(TestTimeout);

            this.consumerMock
                .Setup(c => c.Commit(It.Is<IReadOnlyCollection<TopicPartitionOffset>>(l => l.SequenceEqual(expectedOffsets))))
                .Callback((IEnumerable<TopicPartitionOffset> _) => ready.SetResult());

            // Act
            this.offsetCommitter.MarkAsProcessed(
                new KafkaFlow.TopicPartitionOffset(
                    this.topicPartition.Topic,
                    this.topicPartition.Partition,
                    1));

            await ready.Task;

            // Assert
            this.consumerMock.VerifyAll();
        }

        [TestMethod]
        public async Task PendingOffsetsState_ShouldExecuteHandlers()
        {
            // Arrange
            var ready = new TaskCompletionSource().WithTimeout(TestTimeout);

            var committer = new OffsetCommitter(
                this.consumerMock.Object,
                Mock.Of<IDependencyResolver>(),
                Mock.Of<ILogHandler>());

            committer.PendingOffsetsStatisticsHandlers.Add(new((_, _) => ready.TrySetResult(), TimeSpan.FromMilliseconds(100)));

            await committer.StartAsync();

            // Act
            committer.MarkAsProcessed(
                new KafkaFlow.TopicPartitionOffset(
                    this.topicPartition.Topic,
                    this.topicPartition.Partition,
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
            var expectedOffsets = new[] { new TopicPartitionOffset(this.topicPartition, new Offset(2)) };

            var ready = new TaskCompletionSource().WithTimeout(TestTimeout);
            var hasThrown = false;

            this.consumerMock
                .Setup(c => c.Commit(It.Is<IReadOnlyCollection<TopicPartitionOffset>>(l => l.SequenceEqual(expectedOffsets))))
                .Callback(
                    (IEnumerable<TopicPartitionOffset> _) =>
                    {
                        if (!hasThrown)
                        {
                            hasThrown = true;
                            throw new InvalidOperationException();
                        }

                        ready.TrySetResult();
                    });

            // Act
            this.offsetCommitter.MarkAsProcessed(
                new KafkaFlow.TopicPartitionOffset(
                    this.topicPartition.Topic,
                    this.topicPartition.Partition,
                    1));

            await ready.Task;

            // Assert
            this.consumerMock.Verify(
                c => c.Commit(It.Is<IReadOnlyCollection<TopicPartitionOffset>>(l => l.SequenceEqual(expectedOffsets))),
                Times.Exactly(2));
        }
    }
}
