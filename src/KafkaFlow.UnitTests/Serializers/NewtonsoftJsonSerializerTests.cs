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
    public class NewtonsoftJsonSerializerTests
    {
        private readonly Mock<ISerializerContext> contextMock = new ();
        private readonly NewtonsoftJsonSerializer serializer = new ();

        private readonly Fixture fixture = new();

        [TestMethod]
        public async Task SerializeAsync_ValidPayload_JsonByteArrayGenerated()
        {
            // Arrange
            var message = this.fixture.Create<TestMessage>();
            using var output = new MemoryStream();

            // Act
            await this.serializer.SerializeAsync(message, output, this.contextMock.Object);

            // Assert
            output.Length.Should().BeGreaterThan(0);
            output.Position.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public async Task DeserializeAsync_ValidPayload_ObjectGenerated()
        {
            // Arrange
            var message = this.fixture.Create<TestMessage>();
            using var input = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)));

            // Act
            var result = await this.serializer.DeserializeAsync(input, typeof(TestMessage), this.contextMock.Object);

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
