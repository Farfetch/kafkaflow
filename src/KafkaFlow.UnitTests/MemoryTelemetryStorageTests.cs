namespace KafkaFlow.UnitTests
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using KafkaFlow.Admin;
    using KafkaFlow.Admin.Messages;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class MemoryTelemetryStorageTests
    {
        private Mock<IDateTimeProvider> dateTimeProviderMock;

        private MemoryTelemetryStorage target;

        [TestInitialize]
        public void Setup()
        {
            this.dateTimeProviderMock = new(MockBehavior.Strict);

            this.dateTimeProviderMock
                .SetupGet(x => x.MinValue)
                .Returns(DateTime.MinValue);

            this.target = new(
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1),
                this.dateTimeProviderMock.Object);
        }

        [TestMethod]
        public void Get_NoItems_ReturnsEmpty()
        {
            // Act
            var metrics = this.target.Get();

            // Assert
            metrics.Should().BeEmpty();
        }

        [TestMethod]
        public void Put_OneItem_ReturnsOneItem()
        {
            // Arrange
            var now = DateTime.Now;

            this.dateTimeProviderMock
                .SetupGet(x => x.UtcNow)
                .Returns(now);

            // Act
            this.target.Put(new ConsumerTelemetryMetric { SentAt = now });

            // Assert
            this.target.Get().Should().HaveCount(1);
        }

        [TestMethod]
        public void PutTwoItems_SameInstanceGroupConsumer_ReplaceOlder()
        {
            // Arrange
            var now = DateTime.Now;

            this.dateTimeProviderMock
                .SetupGet(x => x.UtcNow)
                .Returns(now);

            var metric1 = new ConsumerTelemetryMetric
            {
                InstanceName = Guid.NewGuid().ToString(),
                GroupId = Guid.NewGuid().ToString(),
                ConsumerName = Guid.NewGuid().ToString(),
                SentAt = now,
            };

            var metric2 = new ConsumerTelemetryMetric
            {
                InstanceName = metric1.InstanceName,
                GroupId = metric1.GroupId,
                ConsumerName = metric1.ConsumerName,
                SentAt = now,
            };

            // Act
            this.target.Put(metric1);
            this.target.Put(metric2);

            // Assert
            this.target.Get().Should().HaveCount(1);
            this.target.Get().First().Should().Be(metric2);
        }

        [TestMethod]
        public void PutTwoItems_DifferentInstanceGroupConsumer_ReturnsTwo()
        {
            // Arrange
            var now = DateTime.Now;

            this.dateTimeProviderMock
                .SetupGet(x => x.UtcNow)
                .Returns(now);

            var metric1 = new ConsumerTelemetryMetric
            {
                InstanceName = Guid.NewGuid().ToString(),
                GroupId = Guid.NewGuid().ToString(),
                ConsumerName = Guid.NewGuid().ToString(),
                SentAt = now,
            };

            var metric2 = new ConsumerTelemetryMetric
            {
                InstanceName = Guid.NewGuid().ToString(),
                GroupId = Guid.NewGuid().ToString(),
                ConsumerName = Guid.NewGuid().ToString(),
                SentAt = now,
            };

            // Act
            this.target.Put(metric1);
            this.target.Put(metric2);

            // Assert
            this.target.Get().Should().HaveCount(2);
        }

        [TestMethod]
        public void PutTwoItems_ExpiryOne_ReturnsOne()
        {
            // Arrange
            var now = new DateTime(2000, 01, 01);

            this.dateTimeProviderMock
                .SetupGet(x => x.Now)
                .Returns(now);

            var metric1 = new ConsumerTelemetryMetric
            {
                InstanceName = Guid.NewGuid().ToString(),
                GroupId = Guid.NewGuid().ToString(),
                ConsumerName = Guid.NewGuid().ToString(),
                SentAt = now,
            };

            this.target.Put(metric1);

            var metric2 = new ConsumerTelemetryMetric
            {
                InstanceName = Guid.NewGuid().ToString(),
                GroupId = Guid.NewGuid().ToString(),
                ConsumerName = Guid.NewGuid().ToString(),
                SentAt = now.AddSeconds(5),
            };

            this.dateTimeProviderMock
                .SetupGet(x => x.Now)
                .Returns(now.AddSeconds(2));

            // Act
            this.target.Put(metric2);

            // Assert
            this.target.Get().Should().HaveCount(1);
            this.target.Get().First().Should().Be(metric2);
        }
    }
}
