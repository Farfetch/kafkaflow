using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using KafkaFlow.Serializer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KafkaFlow.UnitTests.Serializers
{
    [TestClass]
    public class JsonCoreSerializerTests
    {
        private readonly Mock<ISerializerContext> _contextMock = new();

        [TestMethod]
        public async Task SerializeAsync_PreventEscapeOfAccentedCharacter_SerializedObjectDoesNotHaveAccentedCharacterEscaped()
        {
            // Arrange
            var message = new TestMessage { Text = "Café Façade" };
            using var output = new MemoryStream();

            var writerOptions = new JsonWriterOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

            var target = new JsonCoreSerializer(writerOptions);

            // Act
            await target.SerializeAsync(message, output, _contextMock.Object);

            // Assert
            var result = GetStreamText(output);
            result.Should().Contain("Café Façade");
        }

        private static string GetStreamText(MemoryStream output)
        {
            output.Position = 0;
            var reader = new StreamReader(output);
            return reader.ReadToEnd();
        }

        private class TestMessage
        {
            public string Text { get; set; }
        }
    }
}
