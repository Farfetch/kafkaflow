using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using KafkaFlow.Middlewares.Serializer;
using KafkaFlow.Middlewares.Serializer.Resolvers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KafkaFlow.UnitTests.Serializers;

[TestClass]
public class SerializerConsumerMiddlewareTests
{
    private Mock<IMessageContext> _contextMock;
    private Mock<IDeserializer> _deserializerMock;
    private Mock<IMessageTypeResolver> _typeResolverMock;

    private bool _nextCalled;

    private DeserializerConsumerMiddleware _target;

    [TestInitialize]
    public void Setup()
    {
        _contextMock = new Mock<IMessageContext>();
        _deserializerMock = new Mock<IDeserializer>();
        _typeResolverMock = new Mock<IMessageTypeResolver>();

        _target = new DeserializerConsumerMiddleware(
            _deserializerMock.Object,
            _typeResolverMock.Object);
    }

    [TestMethod]
    public async Task Invoke_NullMessageType_ReturnWithoutCallingNext()
    {
        // Arrange
        _contextMock
            .SetupGet(x => x.Message)
            .Returns(new Message(new byte[1], new byte[1]));

        _typeResolverMock
            .Setup(x => x.OnConsumeAsync(_contextMock.Object))
            .ReturnsAsync((Type)null);

        // Act
        await _target.Invoke(_contextMock.Object, _ => this.SetNextCalled());

        // Assert
        _nextCalled.Should().BeFalse();
        _typeResolverMock.VerifyAll();
        _contextMock.Verify(x => x.SetMessage(It.IsAny<object>(), It.IsAny<object>()), Times.Never);
        _deserializerMock.Verify(
            x => x.DeserializeAsync(It.IsAny<Stream>(), It.IsAny<Type>(), It.IsAny<ISerializerContext>()),
            Times.Never);
    }

    [TestMethod]
    public async Task Invoke_NullMessage_CallNext()
    {
        // Arrange
        _contextMock
            .SetupGet(x => x.Message)
            .Returns(new Message(null, null));

        // Act
        await _target.Invoke(_contextMock.Object, _ => this.SetNextCalled());

        // Assert
        _nextCalled.Should().BeTrue();
        _deserializerMock.Verify(
            x => x.DeserializeAsync(It.IsAny<Stream>(), It.IsAny<Type>(), It.IsAny<ISerializerContext>()),
            Times.Never);
        _typeResolverMock.Verify(x => x.OnConsumeAsync(It.IsAny<IMessageContext>()), Times.Never);
    }

    [TestMethod]
    public void Invoke_NotByteArrayMessage_ThrowsInvalidOperationException()
    {
        // Arrange
        _contextMock
            .SetupGet(x => x.Message)
            .Returns(new Message(null, new TestMessage()));

        // Act
        Func<Task> act = () => _target.Invoke(_contextMock.Object, _ => this.SetNextCalled());

        // Assert
        act.Should().ThrowAsync<InvalidOperationException>();
        _nextCalled.Should().BeFalse();
        _contextMock.Verify(x => x.SetMessage(It.IsAny<object>(), It.IsAny<object>()), Times.Never);
        _deserializerMock.Verify(
            x => x.DeserializeAsync(It.IsAny<Stream>(), It.IsAny<Type>(), It.IsAny<ISerializerContext>()),
            Times.Never);
        _typeResolverMock.Verify(x => x.OnConsumeAsync(It.IsAny<IMessageContext>()), Times.Never);
    }

    [TestMethod]
    public async Task Invoke_ValidMessage_Deserialize()
    {
        // Arrange
        var messageType = typeof(TestMessage);
        var rawKey = new byte[1];
        var rawValue = new byte[1];
        var rawMessage = new Message(rawKey, rawValue);
        var deserializedMessage = new TestMessage();

        var consumerContext = new Mock<IConsumerContext>();
        consumerContext.SetupGet(x => x.Topic).Returns("test-topic");

        var transformedContextMock = new Mock<IMessageContext>();
        IMessageContext resultContext = null;

        _contextMock
            .SetupGet(x => x.Message)
            .Returns(rawMessage);

        _contextMock
            .Setup(x => x.SetMessage(rawKey, deserializedMessage))
            .Returns(transformedContextMock.Object);

        _typeResolverMock
            .Setup(x => x.OnConsumeAsync(_contextMock.Object))
            .ReturnsAsync(messageType);

        _deserializerMock
            .Setup(x => x.DeserializeAsync(It.IsAny<Stream>(), messageType, It.IsAny<ISerializerContext>()))
            .ReturnsAsync(deserializedMessage);

        _contextMock
            .SetupGet(x => x.ConsumerContext)
            .Returns(consumerContext.Object);

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
        _deserializerMock.VerifyAll();
        _typeResolverMock.VerifyAll();
    }

    private Task SetNextCalled()
    {
        _nextCalled = true;
        return Task.CompletedTask;
    }

    private class TestMessage
    {
    }
}
