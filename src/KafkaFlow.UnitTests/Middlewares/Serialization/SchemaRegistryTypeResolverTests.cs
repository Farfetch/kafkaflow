namespace KafkaFlow.UnitTests.Middlewares.Serialization
{
    using System;
    using System.Buffers.Binary;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class SchemaRegistryTypeResolverTests
    {
        private readonly Mock<IMessageContext> messageContextMock;
        private readonly Mock<ISchemaRegistryTypeNameResolver> schemaRegistryTypeNameResolverMock;
        private readonly SchemaRegistryTypeResolver schemaRegistryTypeResolver;
        private readonly byte[] messageKey = new byte[] { 0x18, 0x19 };
        private readonly byte[] messageValue = new byte[] { 0x20, 0x21, 0x22, 0x23, 0x24, 0x25 };

        public SchemaRegistryTypeResolverTests()
        {
            this.messageContextMock = new Mock<IMessageContext>();
            this.messageContextMock.Setup(context => context.Message).Returns(new Message(messageKey, messageValue));
            this.schemaRegistryTypeNameResolverMock = new Mock<ISchemaRegistryTypeNameResolver>();
            this.schemaRegistryTypeResolver = new SchemaRegistryTypeResolver(this.schemaRegistryTypeNameResolverMock.Object);
        }

        [TestMethod]
        public async Task OnConsumeAsync_WhenCalledTwice_TypeIsResolvedOnceThenTypeIsLoadedFromCache()
        {
            // Arrange
            var expectedSchemaId = BinaryPrimitives.ReadInt32BigEndian(
                this.messageValue.AsSpan().Slice(1, 4));

            this.schemaRegistryTypeNameResolverMock.Setup(
                resolver => resolver.ResolveAsync(expectedSchemaId)).ReturnsAsync(typeof(SchemaRegistryTypeResolverTests).FullName);

            // Act
            await this.schemaRegistryTypeResolver.OnConsumeAsync(messageContextMock.Object);
            var type = await this.schemaRegistryTypeResolver.OnConsumeAsync(messageContextMock.Object);

            // Assert
            this.schemaRegistryTypeNameResolverMock.Verify(resolver => resolver.ResolveAsync(expectedSchemaId), Times.Once);
            var expectedObject = (SchemaRegistryTypeResolverTests)Activator.CreateInstance(type);
            expectedObject.Should().NotBeNull();
        }
    }
}
