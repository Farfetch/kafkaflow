using System.Text;
using Confluent.Kafka;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KafkaFlow.UnitTests
{
    [TestClass]
    public class MessageHeadersTests
    {
        private const string Key = "abc";
        private const string StrValue = "123";
        private readonly byte[] _value = Encoding.UTF8.GetBytes("123");

        [TestMethod]
        public void Add_WithKeyNotNull_ShouldAddValueCorrectly()
        {
            // Arrange
            var header = new MessageHeaders();

            // Act
            header.Add(Key, _value);

            // Assert
            header[Key].Should().BeEquivalentTo(_value);
        }

        [TestMethod]
        public void GetKafkaHeader_ShouldReturnKafkaHeaders()
        {
            // Arrange
            var kafkaHeaders = new Headers { { Key, _value } };
            var messageHeaders = new MessageHeaders(kafkaHeaders);

            // Act
            var result = messageHeaders.GetKafkaHeaders();

            // Assert
            result.Should().BeEquivalentTo(kafkaHeaders);
        }

        [TestMethod]
        public void SetString_WithValueNotNull_ShouldAddValueCorrectly()
        {
            // Arrange
            var header = new MessageHeaders();

            // Act
            header.SetString(Key, StrValue);

            // Assert
            header.GetString(Key).Should().BeEquivalentTo(StrValue);
        }
    }
}
