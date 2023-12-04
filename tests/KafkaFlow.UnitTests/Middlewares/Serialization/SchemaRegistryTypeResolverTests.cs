using System;
using System.Buffers.Binary;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KafkaFlow.UnitTests.Middlewares.Serialization;

[TestClass]
public class SchemaRegistryTypeResolverTests
{
    private readonly Mock<IMessageContext> _messageContextMock;
    private readonly Mock<ISchemaRegistryTypeNameResolver> _schemaRegistryTypeNameResolverMock;
    private readonly SchemaRegistryTypeResolver _schemaRegistryTypeResolver;
    private readonly byte[] _messageKey = new byte[] { 0x18, 0x19 };
    private readonly byte[] _messageValue = new byte[] { 0x20, 0x21, 0x22, 0x23, 0x24, 0x25 };

    public SchemaRegistryTypeResolverTests()
    {
        _messageContextMock = new Mock<IMessageContext>();
        _messageContextMock.Setup(context => context.Message).Returns(new Message(_messageKey, _messageValue));
        _schemaRegistryTypeNameResolverMock = new Mock<ISchemaRegistryTypeNameResolver>();
        _schemaRegistryTypeResolver = new SchemaRegistryTypeResolver(_schemaRegistryTypeNameResolverMock.Object);
    }

    [TestMethod]
    public async Task OnConsumeAsync_WhenCalledTwice_TypeIsResolvedOnceThenTypeIsLoadedFromCache()
    {
        // Arrange
        var expectedSchemaId = BinaryPrimitives.ReadInt32BigEndian(
            _messageValue.AsSpan().Slice(1, 4));

        _schemaRegistryTypeNameResolverMock.Setup(
            resolver => resolver.ResolveAsync(expectedSchemaId)).ReturnsAsync(typeof(SchemaRegistryTypeResolverTests).FullName);

        // Act
        await _schemaRegistryTypeResolver.OnConsumeAsync(_messageContextMock.Object);
        var type = await _schemaRegistryTypeResolver.OnConsumeAsync(_messageContextMock.Object);

        // Assert
        _schemaRegistryTypeNameResolverMock.Verify(resolver => resolver.ResolveAsync(expectedSchemaId), Times.Once);
        var expectedObject = (SchemaRegistryTypeResolverTests)Activator.CreateInstance(type);
        expectedObject.Should().NotBeNull();
    }
}
