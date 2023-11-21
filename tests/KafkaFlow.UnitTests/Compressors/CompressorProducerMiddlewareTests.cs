using System;
using System.Threading.Tasks;
using FluentAssertions;

using KafkaFlow.Middlewares.Compressor;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KafkaFlow.UnitTests.Compressors
{
    [TestClass]
    public class CompressorProducerMiddlewareTests
    {
        private Mock<IMessageContext> _contextMock;
        private Mock<ICompressor> _compressorMock;

        private CompressorProducerMiddleware _target;

        [TestInitialize]
        public void Setup()
        {
            _contextMock = new Mock<IMessageContext>();
            _compressorMock = new Mock<ICompressor>();

            _target = new CompressorProducerMiddleware(_compressorMock.Object);
        }

        [TestMethod]
        public async Task Invoke_InvalidMessage_Throws()
        {
            // Arrange
            var uncompressedMessage = new Message(new byte[1], new object());
            IMessageContext resultContext = null;

            _contextMock
                .SetupGet(x => x.Message)
                .Returns(uncompressedMessage);

            // Act
            Func<Task> act = () => _target.Invoke(
                _contextMock.Object,
                ctx =>
                {
                    resultContext = ctx;
                    return Task.CompletedTask;
                });

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
            resultContext.Should().BeNull();
            _contextMock.Verify(x => x.SetMessage(It.IsAny<object>(), It.IsAny<object>()), Times.Never);
            _compressorMock.Verify(x => x.Compress(It.IsAny<byte[]>()), Times.Never);
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

            _contextMock
                .SetupGet(x => x.Message)
                .Returns(uncompressedMessage);

            _compressorMock
                .Setup(x => x.Compress(uncompressedValue))
                .Returns(compressedValue);

            _contextMock
                .Setup(x => x.SetMessage(uncompressedMessage.Key, compressedValue))
                .Returns(transformedContextMock.Object);

            // Act
            await _target.Invoke(
                _contextMock.Object,
                ctx =>
                {
                    resultContext = ctx;
                    return Task.CompletedTask;
                });

            // Assert
            resultContext.Should().NotBeNull();
            resultContext.Should().Be(transformedContextMock.Object);
            _contextMock.VerifyAll();
            _compressorMock.VerifyAll();
        }
    }
}
