namespace KafkaFlow.UnitTests.Middlewares.Serialization
{
    using System.Threading.Tasks;
    using Confluent.SchemaRegistry;
    using FluentAssertions;
    using KafkaFlow.Serializer.SchemaRegistry;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Newtonsoft.Json;

    [TestClass]
    public class ConfluentAvroTypeNameResolverTests
    {
        private readonly Mock<ISchemaRegistryClient> schemaRegistryClient;
        private readonly ConfluentAvroTypeNameResolver schemaRegistryTypeResolver;

        public ConfluentAvroTypeNameResolverTests()
        {
            this.schemaRegistryClient = new Mock<ISchemaRegistryClient>();
            this.schemaRegistryTypeResolver = new ConfluentAvroTypeNameResolver(this.schemaRegistryClient.Object);
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

            this.schemaRegistryClient.Setup(client => client.GetSchemaAsync(schemaId, null))
                .ReturnsAsync(schema);

            // Act
            var avroFields = await this.schemaRegistryTypeResolver.ResolveAsync(schemaId);

            // Assert
            avroFields.Should().Be($"{schemaObj.NameSpace}.{schemaObj.Name}");
        }
    }
}
