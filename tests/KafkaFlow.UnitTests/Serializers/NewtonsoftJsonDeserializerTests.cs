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

namespace KafkaFlow.UnitTests.Serializers;

[TestClass]
public class NewtonsoftJsonDeserializerTests
{
    private readonly Mock<ISerializerContext> _contextMock = new();
    private readonly NewtonsoftJsonDeserializer _deserializer = new();

    private readonly Fixture _fixture = new();

    [TestMethod]
    public async Task DeserializeAsync_ValidPayload_ObjectGenerated()
    {
        // Arrange
        var message = _fixture.Create<TestMessage>();
        using var input = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)));

        // Act
        var result = await _deserializer.DeserializeAsync(input, typeof(TestMessage), _contextMock.Object);

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
