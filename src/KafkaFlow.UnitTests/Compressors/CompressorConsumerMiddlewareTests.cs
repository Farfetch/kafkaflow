namespace KafkaFlow.UnitTests.Compressors
{
    using System;
    using System.Threading.Tasks;
    using Compressor;
    using FluentAssertions;
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
                .Returns(new object());

            // Act
            Func<Task> act = () => this.target.Invoke(this.contextMock.Object, c => this.SetNextCalled());

            // Assert
            act.Should().Throw<InvalidOperationException>();
            this.nextCalled.Should().BeFalse();
            this.contextMock.Verify(x => x.TransformMessage(It.IsAny<object>()), Times.Never);
            this.compressorMock.Verify(x => x.Decompress(It.IsAny<byte[]>()), Times.Never);
        }

        [TestMethod]
        public async Task Invoke_ValidMessage_CallNext()
        {
            // Arrange
            var rawMessage = new byte[1];
            var decompressed = new byte[1];

            this.contextMock
                .SetupGet(x => x.Message)
                .Returns(rawMessage);

            this.compressorMock
                .Setup(x => x.Decompress(rawMessage))
                .Returns(decompressed);

            // Act
            await this.target.Invoke(this.contextMock.Object, c => this.SetNextCalled());

            // Assert
            this.nextCalled.Should().BeTrue();
            this.contextMock.Verify(x => x.TransformMessage(decompressed));
            this.compressorMock.VerifyAll();
        }

        private Task SetNextCalled()
        {
            this.nextCalled = true;
            return Task.CompletedTask;
        }
    }
}
