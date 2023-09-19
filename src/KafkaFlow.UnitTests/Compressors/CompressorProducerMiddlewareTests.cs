namespace KafkaFlow.UnitTests.Compressors
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;

    using KafkaFlow.Middlewares.Compressor;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class CompressorProducerMiddlewareTests
    {
        private Mock<IMessageContext> contextMock;
        private Mock<ICompressor> compressorMock;

        private CompressorProducerMiddleware target;

        [TestInitialize]
        public void Setup()
        {
            this.contextMock = new Mock<IMessageContext>();
            this.compressorMock = new Mock<ICompressor>();

            this.target = new CompressorProducerMiddleware(this.compressorMock.Object);
        }

        [TestMethod]
        public async Task Invoke_InvalidMessage_Throws()
        {
            // Arrange
            var uncompressedMessage = new Message(new byte[1], new object());
            IMessageContext resultContext = null;

            this.contextMock
                .SetupGet(x => x.Message)
                .Returns(uncompressedMessage);

            // Act
            Func<Task> act = () => this.target.Invoke(
                this.contextMock.Object,
                ctx =>
                {
                    resultContext = ctx;
                    return Task.CompletedTask;
                });

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
            resultContext.Should().BeNull();
            this.contextMock.Verify(x => x.SetMessage(It.IsAny<object>(), It.IsAny<object>()), Times.Never);
            this.compressorMock.Verify(x => x.Compress(It.IsAny<byte[]>()), Times.Never);
        }

        [TestMethod]
        public async Task Invoke_ValidMessage_Compress()
        {
            // Arrange
            var uncompressedValue = new byte[1];
            var compressedValue = new byte[1];
            var uncompressedMessage = new Message(new byte[1], uncompressedValue);

            var transformedContextMock = new Mock<IMessageContext>();
            IMessageContext resultContext = null;

            this.contextMock
                .SetupGet(x => x.Message)
                .Returns(uncompressedMessage);

            this.compressorMock
                .Setup(x => x.Compress(uncompressedValue))
                .Returns(compressedValue);

            this.contextMock
                .Setup(x => x.SetMessage(uncompressedMessage.Key, compressedValue))
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
    }
}
