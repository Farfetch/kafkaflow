namespace KafkaFlow.Client.Tests
{
    using System.Linq;
    using AutoFixture;
    using FluentAssertions;
    using KafkaFlow.Client.Protocol.Streams;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MemoryWriterTests
    {
        private readonly Fixture fixture = new();

        private MemoryWriter target;

        [TestInitialize]
        public void Setup()
        {
            this.target = new(16);
        }

        [TestMethod]
        public void Write_ArrayOfBytes_SameAsArray()
        {
            // Arrange
            var data = this.fixture
                .CreateMany<byte>(128)
                .ToArray();

            // Act
            this.target.Write(data);

            // Assert
            this.target.Length.Should().Be(data.Length);
            this.target.Position.Should().Be(data.Length);
            this.target.Should().BeEquivalentTo(data);
        }

        [TestMethod]
        public void WriteByte_WriteAByte_TheWrittenByte()
        {
            // Arrange
            var data = this.fixture.Create<byte>();

            // Act
            this.target.WriteByte(data);

            // Assert
            this.target.Length.Should().Be(1);
            this.target.Position.Should().Be(1);
            this.target[0].Should().Be(data);
        }

        [TestMethod]
        public void CopyTo_OtherMemoryWritter_CopyAllData()
        {
            // Arrange
            var data = this.fixture.CreateMany<byte>(128).ToArray();
            var writer = new MemoryWriter(32);

            this.target.Write(data);
            this.target.Position = 0;

            // Act
            this.target.CopyTo(writer);

            // Assert
            writer.Length.Should().Be(data.Length);
            writer.Position.Should().Be(data.Length);
            writer.Should().BeEquivalentTo(data);
        }

        // [TestMethod]
        // public void GetSpan_WriteArrayOfBytes_SameAsArray()
        // {
        //     // Arrange
        //     var data = this.fixture
        //         .CreateMany<byte>(4)
        //         .ToArray();
        //
        //     // Act
        //     data.AsSpan().CopyTo(this.target.GetSpan(4));
        //
        //     // Assert
        //     this.target.Length.Should().Be(data.Length);
        //     this.target.Position.Should().Be(data.Length);
        //     this.target.Should().BeEquivalentTo(data);
        // }
    }
}
