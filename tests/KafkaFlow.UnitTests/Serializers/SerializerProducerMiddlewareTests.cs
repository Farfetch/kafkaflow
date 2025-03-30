using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using KafkaFlow.Middlewares.Serializer;
using KafkaFlow.Middlewares.Serializer.Resolvers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KafkaFlow.UnitTests.Serializers;

[TestClass]
public class SerializerProducerMiddlewareTests
{
    private readonly Fixture _fixture = new();

    private Mock<IMessageContext> _contextMock;
    private Mock<ISerializer> _serializerMock;
    private Mock<IMessageTypeResolver> _typeResolverMock;

    private SerializerProducerMiddleware _target;

    [TestInitialize]
    public void Setup()
    {
        _contextMock = new Mock<IMessageContext>();
        _serializerMock = new Mock<ISerializer>();
        _typeResolverMock = new Mock<IMessageTypeResolver>();

        _target = new SerializerProducerMiddleware(
            _serializerMock.Object,
            _typeResolverMock.Object);
    }

    [TestMethod]
    public async Task Invoke_ValidMessage_Serialize()
    {
        // Arrange
        var rawMessage = _fixture.Create<byte[]>();
        var key = new object();
        var deserializedMessage = new Message(key, new TestMessage());
        IMessageContext resultContext = null;
        var producerContext = new Mock<IProducerContext>();
        producerContext.SetupGet(x => x.Topic).Returns("test-topic");

        var transformedContextMock = new Mock<IMessageContext>();

        _contextMock
            .SetupGet(x => x.Message)
            .Returns(deserializedMessage);

        _typeResolverMock.Setup(x => x.OnProduceAsync(_contextMock.Object));

        _serializerMock
            .Setup(
                x => x.SerializeAsync(
                    deserializedMessage.Value,
                    It.IsAny<Stream>(),
                    It.IsAny<ISerializerContext>()))
            .Callback((object _, Stream stream, ISerializerContext _) => stream.WriteAsync(rawMessage));

        _contextMock
            .Setup(x => x.SetMessage(key, It.Is<byte[]>(value => value.SequenceEqual(rawMessage))))
            .Returns(transformedContextMock.Object);

        _contextMock
            .SetupGet(x => x.ProducerContext)
            .Returns(producerContext.Object);

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
        _serializerMock.VerifyAll();
        _typeResolverMock.VerifyAll();
    }

    [TestMethod]
    public async Task Invoke_NullMessage_Serialize()
    {
        // Arrange
        byte[] rawMessage = null;
        var key = new object();
        var deserializedMessage = new Message(key, new TestMessage());
        IMessageContext resultContext = null;
        var producerContext = new Mock<IProducerContext>();
        producerContext.SetupGet(x => x.Topic).Returns("test-topic");

        var transformedContextMock = new Mock<IMessageContext>();

        _contextMock
            .SetupGet(x => x.Message)
            .Returns(deserializedMessage);

        _typeResolverMock.Setup(x => x.OnProduceAsync(_contextMock.Object));

        _serializerMock
            .Setup(
                x => x.SerializeAsync(
                    deserializedMessage.Value,
                    It.IsAny<Stream>(),
                    It.IsAny<ISerializerContext>()))
            .Callback((object _, Stream stream, ISerializerContext _) => stream.WriteAsync(rawMessage));

        _contextMock
            .Setup(x => x.SetMessage(key, It.IsAny<IEnumerable<byte>>()))
            .Returns(transformedContextMock.Object);

        _contextMock
            .SetupGet(x => x.ProducerContext)
            .Returns(producerContext.Object);

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
        resultContext.Message.Value.Should().BeNull();
        _contextMock.VerifyAll();
        _serializerMock.VerifyAll();
        _typeResolverMock.VerifyAll();
    }

    [TestMethod]
    public async Task Preserve_null_message_body()
    {
        // Arrange
        var key = new object();
        var deserializedMessage = new Message(key, null);
        IMessageContext resultContext = null;
        var producerContext = new Mock<IProducerContext>();
        producerContext.SetupGet(x => x.Topic).Returns("test-topic");

        var transformedContextMock = new Mock<IMessageContext>();

        _contextMock
            .SetupGet(x => x.Message)
            .Returns(deserializedMessage);

        _typeResolverMock.Setup(x => x.OnProduceAsync(_contextMock.Object));

        _contextMock
            .Setup(x => x.SetMessage(key, It.IsAny<IEnumerable<byte>>()))
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
        resultContext.Message.Value.Should().BeNull();
        _contextMock.VerifyAll();
        _typeResolverMock.VerifyAll();

        // Verify that inner serializer was never called because null is null
        _serializerMock.Verify(
            x => x.SerializeAsync(
                It.IsAny<object>(),
                It.IsAny<Stream>(),
                It.IsAny<ISerializerContext>()),
            Times.Never());
    }


    private class TestMessage
    {
    }
}
