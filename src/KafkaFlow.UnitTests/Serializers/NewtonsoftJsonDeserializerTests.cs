namespace KafkaFlow.UnitTests.Serializers
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using AutoFixture;
    using FluentAssertions;
    using KafkaFlow.Serializer;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Newtonsoft.Json;

    [TestClass]
    public class NewtonsoftJsonDeserializerTests
    {
        private readonly Mock<ISerializerContext> contextMock = new ();
        private readonly NewtonsoftJsonDeserializer deserializer = new ();

        private readonly Fixture fixture = new();

        [TestMethod]
        public async Task DeserializeAsync_ValidPayload_ObjectGenerated()
        {
            // Arrange
            var message = this.fixture.Create<TestMessage>();
            using var input = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)));

            // Act
            var result = await this.deserializer.DeserializeAsync(input, typeof(TestMessage), this.contextMock.Object);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<TestMessage>();
        }

        private class TestMessage
        {
            public int IntegerField { get; set; }

            public string StringField { get; set; }

            public double DoubleField { get; set; }

            public DateTime DateTimeField { get; set; }
        }
    }
}
