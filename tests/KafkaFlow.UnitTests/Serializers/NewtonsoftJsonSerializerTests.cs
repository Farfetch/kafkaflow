using System;
using System.IO;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using KafkaFlow.Serializer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KafkaFlow.UnitTests.Serializers
{
    [TestClass]
    public class NewtonsoftJsonSerializerTests
    {
        private readonly Mock<ISerializerContext> _contextMock = new();
        private readonly NewtonsoftJsonSerializer _serializer = new();

        private readonly Fixture _fixture = new();

        [TestMethod]
        public async Task SerializeAsync_ValidPayload_JsonByteArrayGenerated()
        {
            // Arrange
            var message = _fixture.Create<TestMessage>();
            using var output = new MemoryStream();

            // Act
            await _serializer.SerializeAsync(message, output, _contextMock.Object);

            // Assert
            output.Length.Should().BeGreaterThan(0);
            output.Position.Should().BeGreaterThan(0);
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
