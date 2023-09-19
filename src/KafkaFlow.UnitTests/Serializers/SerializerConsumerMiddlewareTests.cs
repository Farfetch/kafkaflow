namespace KafkaFlow.UnitTests.Serializers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using FluentAssertions;
    using KafkaFlow.Middlewares.Serializer;
    using KafkaFlow.Middlewares.Serializer.Resolvers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class SerializerConsumerMiddlewareTests
    {
        private Mock<IMessageContext> contextMock;
        private Mock<IDeserializer> deserializerMock;
        private Mock<IMessageTypeResolver> typeResolverMock;

        private bool nextCalled;

        private DeserializerConsumerMiddleware target;

        [TestInitialize]
        public void Setup()
        {
            this.contextMock = new Mock<IMessageContext>();
            this.deserializerMock = new Mock<IDeserializer>();
            this.typeResolverMock = new Mock<IMessageTypeResolver>();

            this.target = new DeserializerConsumerMiddleware(
                this.deserializerMock.Object,
                this.typeResolverMock.Object);
        }

        [TestMethod]
        public async Task Invoke_NullMessageType_ReturnWithoutCallingNext()
        {
            // Arrange
            this.contextMock
                .SetupGet(x => x.Message)
                .Returns(new Message(new byte[1], new byte[1]));

            this.typeResolverMock
                .Setup(x => x.OnConsumeAsync(this.contextMock.Object))
                .ReturnsAsync((Type)null);

            // Act
            await this.target.Invoke(this.contextMock.Object, _ => this.SetNextCalled());

            // Assert
            this.nextCalled.Should().BeFalse();
            this.typeResolverMock.VerifyAll();
            this.contextMock.Verify(x => x.SetMessage(It.IsAny<object>(), It.IsAny<object>()), Times.Never);
            this.deserializerMock.Verify(
                x => x.DeserializeAsync(It.IsAny<Stream>(), It.IsAny<Type>(), It.IsAny<ISerializerContext>()),
                Times.Never);
        }

        [TestMethod]
        public async Task Invoke_NullMessage_CallNext()
        {
            // Arrange
            this.contextMock
                .SetupGet(x => x.Message)
                .Returns(new Message(null, null));

            // Act
            await this.target.Invoke(this.contextMock.Object, _ => this.SetNextCalled());

            // Assert
            this.nextCalled.Should().BeTrue();
            this.deserializerMock.Verify(
                x => x.DeserializeAsync(It.IsAny<Stream>(), It.IsAny<Type>(), It.IsAny<ISerializerContext>()),
                Times.Never);
            this.typeResolverMock.Verify(x => x.OnConsumeAsync(It.IsAny<IMessageContext>()), Times.Never);
        }

        [TestMethod]
        public void Invoke_NotByteArrayMessage_ThrowsInvalidOperationException()
        {
            // Arrange
            this.contextMock
                .SetupGet(x => x.Message)
                .Returns(new Message(null, new TestMessage()));

            // Act
            Func<Task> act = () => this.target.Invoke(this.contextMock.Object, _ => this.SetNextCalled());

            // Assert
            act.Should().Throw<InvalidOperationException>();
            this.nextCalled.Should().BeFalse();
            this.contextMock.Verify(x => x.SetMessage(It.IsAny<object>(), It.IsAny<object>()), Times.Never);
            this.deserializerMock.Verify(
                x => x.DeserializeAsync(It.IsAny<Stream>(), It.IsAny<Type>(), It.IsAny<ISerializerContext>()),
                Times.Never);
            this.typeResolverMock.Verify(x => x.OnConsumeAsync(It.IsAny<IMessageContext>()), Times.Never);
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

            this.contextMock
                .SetupGet(x => x.Message)
                .Returns(rawMessage);

            this.contextMock
                .Setup(x => x.SetMessage(rawKey, deserializedMessage))
                .Returns(transformedContextMock.Object);

            this.typeResolverMock
                .Setup(x => x.OnConsumeAsync(this.contextMock.Object))
                .ReturnsAsync(messageType);

            this.deserializerMock
                .Setup(x => x.DeserializeAsync(It.IsAny<Stream>(), messageType, It.IsAny<ISerializerContext>()))
                .ReturnsAsync(deserializedMessage);

            this.contextMock
                .SetupGet(x => x.ConsumerContext)
                .Returns(consumerContext.Object);

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
            this.deserializerMock.VerifyAll();
            this.typeResolverMock.VerifyAll();
        }

        private Task SetNextCalled()
        {
            this.nextCalled = true;
            return Task.CompletedTask;
        }

        private class TestMessage
        {
        }
    }
}
