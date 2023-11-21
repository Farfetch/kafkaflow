using System;
using System.Linq;
using FluentAssertions;
using KafkaFlow.Admin;
using KafkaFlow.Admin.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KafkaFlow.UnitTests
{
    [TestClass]
    public class MemoryTelemetryStorageTests
    {
        private Mock<IDateTimeProvider> _dateTimeProviderMock;

        private MemoryTelemetryStorage _target;

        [TestInitialize]
        public void Setup()
        {
            _dateTimeProviderMock = new();

            _dateTimeProviderMock
                .SetupGet(x => x.MinValue)
                .Returns(DateTime.MinValue);

            _target = new(
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1),
                _dateTimeProviderMock.Object);
        }

        [TestMethod]
        public void Get_NoItems_ReturnsEmpty()
        {
            // Act
            var metrics = _target.Get();

            // Assert
            metrics.Should().BeEmpty();
        }

        [TestMethod]
        public void Put_OneItem_ReturnsOneItem()
        {
            // Arrange
            var now = DateTime.Now;

            _dateTimeProviderMock
                .SetupGet(x => x.UtcNow)
                .Returns(now);

            // Act
            _target.Put(new ConsumerTelemetryMetric { SentAt = now });

            // Assert
            _target.Get().Should().HaveCount(1);
        }

        [TestMethod]
        public void PutTwoItems_SameInstanceGroupConsumer_ReplaceOlder()
        {
            // Arrange
            var now = DateTime.Now;

            _dateTimeProviderMock
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
            _target.Put(metric1);
            _target.Put(metric2);

            // Assert
            _target.Get().Should().HaveCount(1);
            _target.Get().First().Should().Be(metric2);
        }

        [TestMethod]
        public void PutTwoItems_DifferentInstanceGroupConsumer_ReturnsTwo()
        {
            // Arrange
            var now = DateTime.Now;

            _dateTimeProviderMock
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
            _target.Put(metric1);
            _target.Put(metric2);

            // Assert
            _target.Get().Should().HaveCount(2);
        }

        [TestMethod]
        public void PutTwoItems_ExpiryOne_ReturnsOne()
        {
            // Arrange
            var now = new DateTime(2000, 01, 01);

            _dateTimeProviderMock
                .SetupGet(x => x.UtcNow)
                .Returns(now);

            var metric1 = new ConsumerTelemetryMetric
            {
                InstanceName = Guid.NewGuid().ToString(),
                GroupId = Guid.NewGuid().ToString(),
                ConsumerName = Guid.NewGuid().ToString(),
                SentAt = now,
            };

            _target.Put(metric1);

            var metric2 = new ConsumerTelemetryMetric
            {
                InstanceName = Guid.NewGuid().ToString(),
                GroupId = Guid.NewGuid().ToString(),
                ConsumerName = Guid.NewGuid().ToString(),
                SentAt = now.AddSeconds(5),
            };

            _dateTimeProviderMock
                .SetupGet(x => x.UtcNow)
                .Returns(now.AddSeconds(2));

            // Act
            _target.Put(metric2);

            // Assert
            _target.Get().Should().HaveCount(1);
            _target.Get().First().Should().Be(metric2);
        }
    }
}
