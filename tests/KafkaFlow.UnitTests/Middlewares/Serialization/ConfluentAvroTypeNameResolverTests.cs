using System.Threading.Tasks;
using Confluent.SchemaRegistry;
using FluentAssertions;
using KafkaFlow.Serializer.SchemaRegistry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace KafkaFlow.UnitTests.Middlewares.Serialization
{
    [TestClass]
    public class ConfluentAvroTypeNameResolverTests
    {
        private readonly Mock<ISchemaRegistryClient> _schemaRegistryClient;
        private readonly ConfluentAvroTypeNameResolver _schemaRegistryTypeResolver;

        public ConfluentAvroTypeNameResolverTests()
        {
            _schemaRegistryClient = new Mock<ISchemaRegistryClient>();
            _schemaRegistryTypeResolver = new ConfluentAvroTypeNameResolver(_schemaRegistryClient.Object);
        }

        [TestMethod]
        public async Task ResolveAsync_ValidSchemaObject_ReturnsAvroFieldsInCorrectFormat()
        {
            // Arrange
            var schemaId = 420;
            var type = typeof(ConfluentAvroTypeNameResolverTests);
            var schemaObj = new
            {
                Name = type.Name,
                NameSpace = type.Namespace,
            };

            var schema = new Schema(JsonConvert.SerializeObject(schemaObj), SchemaType.Avro);

            _schemaRegistryClient.Setup(client => client.GetSchemaAsync(schemaId, null))
                .ReturnsAsync(schema);

            // Act
            var avroFields = await _schemaRegistryTypeResolver.ResolveAsync(schemaId);

            // Assert
            avroFields.Should().Be($"{schemaObj.NameSpace}.{schemaObj.Name}");
        }
    }
}
