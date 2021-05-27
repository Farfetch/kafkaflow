namespace KafkaFlow.UnitTests.Serializers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class SerializerProducerMiddlewareTests
    {
        private readonly Fixture fixture = new();

        private Mock<IMessageContext> contextMock;
        private Mock<ISerializer> serializerMock;
        private Mock<IMessageTypeResolver> typeResolverMock;

        private SerializerProducerMiddleware target;

        [TestInitialize]
        public void Setup()
        {
            this.contextMock = new Mock<IMessageContext>();
            this.serializerMock = new Mock<ISerializer>();
            this.typeResolverMock = new Mock<IMessageTypeResolver>();

            this.target = new SerializerProducerMiddleware(
                this.serializerMock.Object,
                this.typeResolverMock.Object);
        }

        [TestMethod]
        public async Task Invoke_ValidMessage_Serialize()
        {
            // Arrange
            var rawMessage = this.fixture.Create<byte[]>();
            var key = new object();
            var deserializedMessage = new Message(key, new TestMessage());
            IMessageContext resultContext = null;

            var transformedContextMock = new Mock<IMessageContext>();

            this.contextMock
                .SetupGet(x => x.Message)
                .Returns(deserializedMessage);

            this.typeResolverMock.Setup(x => x.OnProduce(this.contextMock.Object));

            this.serializerMock
                .Setup(
                    x => x.SerializeAsync(
                        deserializedMessage.Value,
                        It.IsAny<Stream>(),
                        It.IsAny<ISerializerContext>()))
                .Callback((object _, Stream stream, ISerializerContext _) => stream.WriteAsync(rawMessage));

            this.contextMock
                .Setup(x => x.SetMessage(key, It.Is<byte[]>(value => value.SequenceEqual(rawMessage))))
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
            this.serializerMock.VerifyAll();
            this.typeResolverMock.VerifyAll();
        }

        private class TestMessage
        {
        }
    }
}
