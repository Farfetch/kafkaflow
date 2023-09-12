namespace KafkaFlow.UnitTests.Middlewares.Serialization
{
    using System;
    using System.Threading.Tasks;
    using Confluent.SchemaRegistry;

    using FluentAssertions;

    using Google.Protobuf;
    using KafkaFlow.Serializer.SchemaRegistry;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ConfluentProtobufTypeNameResolverTests
    {
        private readonly Mock<ISchemaRegistryClient> schemaRegistryClient;
        private readonly ConfluentProtobufTypeNameResolver schemaRegistryTypeResolver;

        public ConfluentProtobufTypeNameResolverTests()
        {
            this.schemaRegistryClient = new Mock<ISchemaRegistryClient>();
            this.schemaRegistryTypeResolver = new ConfluentProtobufTypeNameResolver(this.schemaRegistryClient.Object);
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

            this.schemaRegistryClient.Setup(client => client.GetSchemaAsync(schemaId, "serialized"))
                .ReturnsAsync(new Schema(base64Encoded, SchemaType.Protobuf));

            // Act
            var protoFields = await this.schemaRegistryTypeResolver.ResolveAsync(schemaId);

            // Assert
            protoFields.Should().NotBeNull();
        }
    }
}
