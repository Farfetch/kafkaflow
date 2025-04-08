using System;
using System.Threading.Tasks;
using FluentAssertions;

using KafkaFlow.Middlewares.Compressor;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KafkaFlow.UnitTests.Compressors;

[TestClass]
public class CompressorConsumerMiddlewareTests
{
    private Mock<IMessageContext> _contextMock;
    private Mock<IDecompressor> _decompressorMock;
    private bool _nextCalled;
    private DecompressorConsumerMiddleware _target;

    [TestInitialize]
    public void Setup()
    {
        _contextMock = new Mock<IMessageContext>();
        _decompressorMock = new Mock<IDecompressor>();
        _target = new DecompressorConsumerMiddleware(_decompressorMock.Object);
    }

    [TestMethod]
    public void Invoke_NotByteArrayMessage_ThrowsInvalidOperationException()
    {
        // Arrange
        _contextMock
            .SetupGet(x => x.Message)
            .Returns(new Message(new object(), new object()));

        // Act
        Func<Task> act = () => _target.Invoke(_contextMock.Object, _ => this.SetNextCalled());

        // Assert
        act.Should().ThrowAsync<InvalidOperationException>();
        _nextCalled.Should().BeFalse();
        _contextMock.Verify(x => x.SetMessage(It.IsAny<object>(), It.IsAny<object>()), Times.Never);
        _decompressorMock.Verify(x => x.Decompress(It.IsAny<byte[]>()), Times.Never);
    }

    [TestMethod]
    public async Task Invoke_ValidMessage_CallNext()
    {
        // Arrange
        var compressedMessage = new Message(null, new byte[1]);
        var uncompressedValue = new byte[1];

        var transformedContextMock = new Mock<IMessageContext>();
        IMessageContext resultContext = null;

        _contextMock
            .SetupGet(x => x.Message)
            .Returns(compressedMessage);

        _decompressorMock
            .Setup(x => x.Decompress((byte[])compressedMessage.Value))
            .Returns(uncompressedValue);

        _contextMock
            .Setup(x => x.SetMessage(compressedMessage.Key, uncompressedValue))
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
        _decompressorMock.VerifyAll();
    }

    private Task SetNextCalled()
    {
        _nextCalled = true;
        return Task.CompletedTask;
    }
}
