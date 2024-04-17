using System;
using System.Collections.Generic;
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
    
    [TestMethod]
    public async Task ResolveAsync_SchemaWithPackageOnly_ReturnsTypeNameWithPackageNamespace()
    {
        // Arrange
        // below schema-string is base64 encoded protobuf schema of 'syntax = \"proto3\";\npackage kafkaflow.test;\n\nmessage Person {\n  string name = 1;\n}\n'
        var schemaString = "CgdkZWZhdWx0Eg5rYWZrYWZsb3cudGVzdCIUCgZQZXJzb24SCgoEbmFtZRgBKAliBnByb3RvMw==";
        var schemaId = 420;
        
        _schemaRegistryClient.Setup(client => client.GetSchemaAsync(schemaId, "serialized"))
            .ReturnsAsync(new RegisteredSchema("test", 1, schemaId, schemaString, SchemaType.Protobuf, new List<SchemaReference>()));
        
        // Act
        var actual = await _schemaRegistryTypeResolver.ResolveAsync(schemaId);
        
        // Assert
        actual.Should().Be("kafkaflow.test.Person");
    }
    
    [TestMethod]
    public async Task ResolveAsync_SchemaWithCsharpNamespace_ReturnsTypeNameWithCsharpNamespace()
    {
        // Arrange
        // below schema-string is base64 encoded protobuf schema of 'syntax = \"proto3\";\npackage kafkaflow.test;\n\nmessage Person {\n  string name = 1;\n}\n'
        var schemaString = "CgdkZWZhdWx0Eg5rYWZrYWZsb3cudGVzdCIUCgZQZXJzb24SCgoEbmFtZRgBKAlCEaoCDkthZmthRmxvdy5UZXN0YgZwcm90bzM=";
        var schemaId = 420;
        
        _schemaRegistryClient.Setup(client => client.GetSchemaAsync(schemaId, "serialized"))
            .ReturnsAsync(new RegisteredSchema("test", 1, schemaId, schemaString, SchemaType.Protobuf, new List<SchemaReference>()));
        
        // Act
        var actual = await _schemaRegistryTypeResolver.ResolveAsync(schemaId);
        
        // Assert
        Assert.AreEqual("KafkaFlow.Test.Person", actual);
    }
    
    [TestMethod]
    public async Task ResolveAsync_SchemaWithoutPackageOrNamespace_ReturnsTypeNameWithoutNamespace()
    {
        // Arrange
        // below schema-string is base64 encoded protobuf schema of 'syntax = \"proto3\";\n\nmessage Person {\n  string name = 1;\n}\n'
        var schemaString = "CgdkZWZhdWx0IhQKBlBlcnNvbhIKCgRuYW1lGAEoCWIGcHJvdG8z";
        var schemaId = 420;
        
        _schemaRegistryClient.Setup(client => client.GetSchemaAsync(schemaId, "serialized"))
            .ReturnsAsync(new RegisteredSchema("test", 1, schemaId, schemaString, SchemaType.Protobuf, new List<SchemaReference>()));
        
        // Act
        var actual = await _schemaRegistryTypeResolver.ResolveAsync(schemaId);
        
        // Assert
        Assert.AreEqual("Person", actual);
    }
}
