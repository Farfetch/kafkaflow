namespace KafkaFlow.UnitTests.Serializers
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    internal class SerializerProducerMiddlewareTests
    {
        private Mock<IMessageContext> contextMock;
        private Mock<IMessageSerializer> serializerMock;
        private Mock<IMessageTypeResolver> typeResolverMock;

        private SerializerProducerMiddleware target;

        [TestInitialize]
        public void Setup()
        {
            this.contextMock = new Mock<IMessageContext>();
            this.serializerMock = new Mock<IMessageSerializer>();
            this.typeResolverMock = new Mock<IMessageTypeResolver>();

            this.target = new SerializerProducerMiddleware(
                this.serializerMock.Object,
                this.typeResolverMock.Object);
        }

        [TestMethod]
        public async Task Invoke_ValidMessage_Serialize()
        {
            // Arrange
            var rawMessage = new byte[1];
            var key = new object();
            var deserializedMessage = new Message(key, new TestMessage());
            IMessageContext resultContext = null;

            var transformedContextMock = new Mock<IMessageContext>();

            this.contextMock
                .SetupGet(x => x.Message)
                .Returns(deserializedMessage);

            this.typeResolverMock.Setup(x => x.OnProduce(this.contextMock.Object));

            this.serializerMock
                .Setup(x => x.Serialize(deserializedMessage.Value))
                .Returns(rawMessage);

            this.contextMock
                .Setup(x => x.TransformMessage(key, rawMessage))
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
