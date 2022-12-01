namespace KafkaFlow.UnitTests.Serializers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using AutoFixture;
    using FluentAssertions;
    using KafkaFlow.Serializer;
    using MessagePack.Resolvers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class MessagePackSerializerTests
    {
        private static readonly MessagePack.MessagePackSerializerOptions MessagePackOptions = MessagePack.MessagePackSerializerOptions
                                                                                                .Standard
                                                                                                .WithResolver(CompositeResolver.Create(
                                                                                                                NativeDateTimeResolver.Instance,
                                                                                                                StandardResolverAllowPrivate.Instance))
                                                                                                .WithCompression(MessagePack.MessagePackCompression.Lz4BlockArray);

        private Mock<ISerializerContext> contextMock = new();

        private MessagePackSerializer serializer = new(MessagePackOptions);

        private Fixture fixture = new();

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
            using var input = new MemoryStream();
            await MessagePack.MessagePackSerializer.SerializeAsync(message.GetType(), input, message, MessagePackOptions);
            input.Seek(0, SeekOrigin.Begin);

            // Act
            var result = await this.serializer.DeserializeAsync(input, typeof(TestMessage), this.contextMock.Object);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<TestMessage>();
        }

        [MessagePack.MessagePackObject]
        private class TestMessage
        {
            [MessagePack.Key(0)]
            public int IntegerField { get; set; }

            [MessagePack.Key(1)]
            public string StringField { get; set; }

            [MessagePack.Key(2)]
            public double DoubleField { get; set; }

            [MessagePack.Key(3)]
            public DateTime DateTimeField { get; set; }
        }
    }
}
