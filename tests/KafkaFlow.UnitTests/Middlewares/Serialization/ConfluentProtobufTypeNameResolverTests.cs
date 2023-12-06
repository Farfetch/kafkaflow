using System;
using System.Threading.Tasks;
using Confluent.SchemaRegistry;
using FluentAssertions;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using KafkaFlow.Serializer.SchemaRegistry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KafkaFlow.UnitTests.Middlewares.Serialization
{
    [TestClass]
    public class ConfluentProtobufTypeNameResolverTests
    {
        private const string MessageTypeName = "TestMessage";

        [TestMethod]
        public async Task ResolveAsync_WithPackage_ReturnTypeName()
        {
            // Arrange
            var schemaRegistryClientMock = CreateSchemaRegistryClientMock(p => p.Package = "TestPackage");
            var resolver = new ConfluentProtobufTypeNameResolver(schemaRegistryClientMock.Object);

            // Act
            var typeName = await resolver.ResolveAsync(1);

            // Assert
            typeName.Should().Be($"TestPackage.{MessageTypeName}");
        }

        [TestMethod]
        public async Task ResolveAsync_NoPackageWithCsharpNamespace_ReturnTypeName()
        {
            // Arrange
            var schemaRegistryClientMock = CreateSchemaRegistryClientMock(p =>
            {
                p.Package = string.Empty;
                p.Options.CsharpNamespace = "TestCsharpNamespace";
            });
            var resolver = new ConfluentProtobufTypeNameResolver(schemaRegistryClientMock.Object);

            // Act
            var typeName = await resolver.ResolveAsync(1);

            // Assert
            typeName.Should().Be($"TestCsharpNamespace.{MessageTypeName}");
        }

        private static Mock<ISchemaRegistryClient> CreateSchemaRegistryClientMock(Action<FileDescriptorProto> configure)
        {
            var protoFields = new FileDescriptorProto
            {
                MessageType =
                {
                    new DescriptorProto
                    {
                        Name = MessageTypeName,
                    },
                },
                Options = new FileOptions(),
            };
            configure(protoFields);

            var schema = new Schema(protoFields.ToByteString().ToBase64(), SchemaType.Protobuf);

            var schemaRegistryClientMock = new Mock<ISchemaRegistryClient>();
            schemaRegistryClientMock
                .Setup(o => o.GetSchemaAsync(1, "serialized"))
                .ReturnsAsync(schema);

            return schemaRegistryClientMock;
        }
    }
}
