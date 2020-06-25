namespace KafkaFlow.UnitTests
{
    using System.Text;
    using Confluent.Kafka;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MessageHeadersTests
    {
        const string key = "abc";
        const string strValue = "123";
        private byte[] value = Encoding.UTF8.GetBytes("123");
        
        [TestMethod]
        public void Add_WithKeyNotNull_ShouldAddValueCorrectly()
        {
            // Arrange
            var header = new MessageHeaders();
            
            // Act
            header.Add(key, this.value);
            
            // Assert
            header[key].Should().BeEquivalentTo(this.value);
        }
        
        [TestMethod]
        public void GetKafkaHeader_ShouldRetunrKafkaHeaders()
        {
            // Arrange
            var kafkaHeaders = new Headers {{key, this.value}};
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
            header.SetString(key, strValue);
            
            // Assert
            header.GetString(key).Should().BeEquivalentTo(strValue);
        }

    }
}