namespace KafkaFlow.UnitTests.Compressors
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using KafkaFlow.Compressor;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class CompressorConsumerMiddlewareTests
    {
        private Mock<IMessageContext> contextMock;
        private Mock<IMessageCompressor> compressorMock;
        private bool nextCalled;
        private CompressorConsumerMiddleware target;

        [TestInitialize]
        public void Setup()
        {
            this.contextMock = new Mock<IMessageContext>();
            this.compressorMock = new Mock<IMessageCompressor>();
            this.target = new CompressorConsumerMiddleware(this.compressorMock.Object);
        }

        [TestMethod]
        public void Invoke_NotByteArrayMessage_ThrowsInvalidOperationException()
        {
            // Arrange
            this.contextMock
                .SetupGet(x => x.Message)
                .Returns(new Message(new object(), new object()));

            // Act
            Func<Task> act = () => this.target.Invoke(this.contextMock.Object, _ => this.SetNextCalled());

            // Assert
            act.Should().Throw<InvalidOperationException>();
            this.nextCalled.Should().BeFalse();
            this.contextMock.Verify(x => x.TransformMessage(It.IsAny<object>(), It.IsAny<object>()), Times.Never);
            this.compressorMock.Verify(x => x.Decompress(It.IsAny<byte[]>()), Times.Never);
        }

        [TestMethod]
        public async Task Invoke_ValidMessage_CallNext()
        {
            // Arrange
            var compressedMessage = new Message(null, new byte[1]);
            var uncompressedValue = new byte[1];

            var transformedContextMock = new Mock<IMessageContext>();
            IMessageContext resultContext = null;

            this.contextMock
                .SetupGet(x => x.Message)
                .Returns(compressedMessage);

            this.compressorMock
                .Setup(x => x.Decompress((byte[]) compressedMessage.Value))
                .Returns(uncompressedValue);

            this.contextMock
                .Setup(x => x.TransformMessage(compressedMessage.Key, uncompressedValue))
                .Returns(transformedContextMock.Object);

            // Act
            await this.target.Invoke(
                this.contextMock.Object,
                ctx =>
                {
                    resultContext = ctx;
                    return Task.CompletedTask;
                });

            // Assert
            resultContext.Should().NotBeNull();
            resultContext.Should().Be(transformedContextMock.Object);
            this.contextMock.VerifyAll();
            this.compressorMock.VerifyAll();
        }

        private Task SetNextCalled()
        {
            this.nextCalled = true;
            return Task.CompletedTask;
        }
    }
}
