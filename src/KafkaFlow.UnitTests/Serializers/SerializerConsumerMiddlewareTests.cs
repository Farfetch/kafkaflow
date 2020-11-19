namespace KafkaFlow.UnitTests.Serializers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using FluentAssertions;
    using KafkaFlow.Serializer;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class SerializerConsumerMiddlewareTests
    {
        private Mock<IMessageContext> contextMock;
        private Mock<IMessageSerializer> serializerMock;
        private Mock<IMessageTypeResolver> typeResolverMock;

        private bool nextCalled;

        private SerializerConsumerMiddleware target;

        [TestInitialize]
        public void Setup()
        {
            this.contextMock = new Mock<IMessageContext>();
            this.serializerMock = new Mock<IMessageSerializer>();
            this.typeResolverMock = new Mock<IMessageTypeResolver>();

            this.target = new SerializerConsumerMiddleware(
                this.serializerMock.Object,
                this.typeResolverMock.Object);
        }

        [TestMethod]
        public async Task Invoke_NullMessageType_ReturnWithoutCallingNext()
        {
            // Arrange
            this.typeResolverMock
                .Setup(x => x.OnConsume(this.contextMock.Object))
                .Returns((Type) null);

            // Act
            await this.target.Invoke(this.contextMock.Object, c => this.SetNextCalled());

            // Assert
            this.nextCalled.Should().BeFalse();
            this.typeResolverMock.VerifyAll();
            this.contextMock.Verify(x => x.TransformMessage(It.IsAny<object>()), Times.Never);
            this.serializerMock.Verify(
                x => x.Deserialize(
                    It.IsAny<Stream>(),
                    It.IsAny<Type>(),
                    It.IsAny<SerializationContext>()),
                Times.Never);
        }

        [TestMethod]
        public async Task Invoke_NullMessage_CallNext()
        {
            // Arrange
            var messageType = typeof(TestMessage);

            this.contextMock
                .SetupGet(x => x.Message)
                .Returns((byte[]) null);

            this.typeResolverMock
                .Setup(x => x.OnConsume(this.contextMock.Object))
                .Returns(messageType);

            // Act
            await this.target.Invoke(this.contextMock.Object, c => this.SetNextCalled());

            // Assert
            this.nextCalled.Should().BeTrue();
            this.serializerMock.Verify(
                x => x.Deserialize(
                    It.IsAny<Stream>(),
                    It.IsAny<Type>(),
                    It.IsAny<SerializationContext>()),
                Times.Never);
            this.typeResolverMock.VerifyAll();
        }

        [TestMethod]
        public void Invoke_NotByteArrayMessage_ThrowsInvalidOperationException()
        {
            // Arrange
            var messageType = typeof(TestMessage);

            this.contextMock
                .SetupGet(x => x.Message)
                .Returns(new TestMessage());

            this.typeResolverMock
                .Setup(x => x.OnConsume(this.contextMock.Object))
                .Returns(messageType);

            // Act
            Func<Task> act = () => this.target.Invoke(this.contextMock.Object, c => this.SetNextCalled());

            // Assert
            act.Should().Throw<InvalidOperationException>();
            this.nextCalled.Should().BeFalse();
            this.contextMock.Verify(x => x.TransformMessage(It.IsAny<object>()), Times.Never);
            this.serializerMock.Verify(
                x => x.Deserialize(
                    It.IsAny<Stream>(),
                    It.IsAny<Type>(),
                    It.IsAny<SerializationContext>()),
                Times.Never);
            this.typeResolverMock.VerifyAll();
        }

        [TestMethod]
        public async Task Invoke_ValidMessage_CallNext()
        {
            // Arrange
            var messageType = typeof(TestMessage);
            var rawMessage = new byte[1];
            var deserializedMessage = new TestMessage();

            this.contextMock
                .SetupGet(x => x.Message)
                .Returns(rawMessage);

            this.typeResolverMock
                .Setup(x => x.OnConsume(this.contextMock.Object))
                .Returns(messageType);

            this.serializerMock
                .Setup(x => x.Deserialize(It.IsAny<Stream>(), messageType, It.IsAny<SerializationContext>()))
                .Returns(deserializedMessage);

            // Act
            await this.target.Invoke(this.contextMock.Object, c => this.SetNextCalled());

            // Assert
            this.nextCalled.Should().BeTrue();
            this.contextMock.Verify(x => x.TransformMessage(deserializedMessage));
            this.serializerMock.VerifyAll();
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
