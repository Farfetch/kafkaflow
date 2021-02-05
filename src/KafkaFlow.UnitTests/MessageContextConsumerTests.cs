namespace KafkaFlow.UnitTests
{
    using System;
    using System.Threading;
    using Confluent.Kafka;
    using FluentAssertions;
    using KafkaFlow.Consumers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MessageContextConsumerTests
    {
        [TestMethod]
        public void MessageTimestamp_ConsumeResultHasMessageTimestamp_ReturnsMessageTimestampFromResult()
        {
            // Arrange
            var expectedMessageTimestamp = new DateTime(2020, 1, 1, 0, 0, 0);

            var consumerResult = new ConsumeResult<byte[], byte[]>
            {
                Message = new Message<byte[], byte[]>
                {
                    Timestamp = new Timestamp(expectedMessageTimestamp)
                }
            };

            var target = new MessageContextConsumer(null, null, consumerResult, CancellationToken.None);

            // Act
            var messageTimestamp = target.MessageTimestamp;

            // Assert
            messageTimestamp.Should().Be(expectedMessageTimestamp);
        }
    }
}
