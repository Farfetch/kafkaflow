using System;
using System.Threading.Tasks;
using Confluent.SchemaRegistry;

using FluentAssertions;

using Google.Protobuf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KafkaFlow.UnitTests.Middlewares.Serialization;

[TestClass]
public class ConfluentProtobufTypeNameResolverTests
{
    private readonly Mock<ISchemaRegistryClient> _schemaRegistryClient;
    private readonly ConfluentProtobufTypeNameResolver _schemaRegistryTypeResolver;

    public ConfluentProtobufTypeNameResolverTests()
    {
        _schemaRegistryClient = new Mock<ISchemaRegistryClient>();
        _schemaRegistryTypeResolver = new ConfluentProtobufTypeNameResolver(_schemaRegistryClient.Object);
    }

    [TestMethod]
    public async Task ResolveAsync_ValidProtobufObject_ReturnsProtoFields()
    {
        // Arrange
        var schemaId = 420;

        var dummyProtobufObj = new DummyProtobufObject
        {
            Field1 = "Field1",
            Field2 = 8,
        };
        var base64Encoded = Convert.ToBase64String(dummyProtobufObj.ToByteArray());

        _schemaRegistryClient.Setup(client => client.GetSchemaAsync(schemaId, "serialized"))
            .ReturnsAsync(new Schema(base64Encoded, SchemaType.Protobuf));

        // Act
        var protoFields = await _schemaRegistryTypeResolver.ResolveAsync(schemaId);

        // Assert
        protoFields.Should().NotBeNull();
    }
}
