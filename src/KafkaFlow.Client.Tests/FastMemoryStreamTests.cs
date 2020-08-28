namespace KafkaFlow.Client.Tests
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using AutoFixture;
    using FluentAssertions;
    using KafkaFlow.Client.Protocol.MemoryManagement;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class FastMemoryStreamTests
    {
        private readonly Fixture fixture = new Fixture();
        private Mock<IFastMemoryManager> managerMock;

        private FastMemoryStream target;

        private const int SegmentSize = 64;

        [TestInitialize]
        public void Setup()
        {
            this.managerMock = new Mock<IFastMemoryManager>();

            this.managerMock
                .Setup(x => x.Allocate(It.IsAny<int>()))
                .Returns<int>(Marshal.AllocHGlobal);

            this.managerMock
                .Setup(x => x.Free(It.IsAny<IntPtr>()))
                .Callback<IntPtr>(Marshal.FreeHGlobal);

            this.target = new FastMemoryStream(this.managerMock.Object, SegmentSize);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(16)]
        [DataRow(SegmentSize)]
        [DataRow(98)]
        [DataRow(SegmentSize * 3)]
        [DataRow(478)]
        [DataRow(1024 * 1024)]
        public void AllocateAndFreeMemory_AllocateCorrectly_FreeCorrectly(int bufferSize)
        {
            // Arrange
            var buffer = new byte[bufferSize];

            this.target.Write(buffer, 0, buffer.Length);

            // Act
            this.target.Dispose();

            // Assert
            this.managerMock.Verify(
                x => x.Allocate(SegmentSize),
                Times.Exactly((int)Math.Ceiling((double)bufferSize / SegmentSize)));

            this.managerMock.Verify(
                x => x.Free(It.IsAny<IntPtr>()),
                Times.Exactly((int)Math.Ceiling((double)bufferSize / SegmentSize)));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(16)]
        [DataRow(SegmentSize)]
        [DataRow(98)]
        [DataRow(SegmentSize * 3)]
        [DataRow(478)]
        [DataRow(1024 * 1024)]
        public void PositionAndLength_EqualToBufferSize(int bufferSize)
        {
            // Arrange
            var buffer = new byte[bufferSize];
            this.target.Write(buffer, 0, buffer.Length);

            // Act
            this.target.Dispose();

            // Assert
            this.target.Position.Should().Be(bufferSize);
            this.target.Length.Should().Be(bufferSize);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(16)]
        [DataRow(SegmentSize)]
        [DataRow(98)]
        [DataRow(SegmentSize * 3)]
        [DataRow(478)]
        public void Write_EqualToBuffer(int bufferSize)
        {
            // Arrange
            var buffer = this.fixture
                .CreateMany<byte>(bufferSize)
                .ToArray();

            // Act
            this.target.Write(buffer, 0, buffer.Length);

            // Assert
            this.target.Should().BeEquivalentTo(buffer);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(16)]
        [DataRow(SegmentSize)]
        [DataRow(98)]
        [DataRow(SegmentSize * 3)]
        [DataRow(478)]
        [DataRow(1024 * 5)]
        public void Read_EqualToBuffer(int bufferSize)
        {
            // Arrange
            var expectedBuffer = this.fixture
                .CreateMany<byte>(bufferSize)
                .ToArray();

            var resultBuffer = new byte[bufferSize];

            this.target.Write(expectedBuffer, 0, expectedBuffer.Length);
            this.target.Position = 0;

            // Act
            this.target.Read(resultBuffer);

            // Assert
            resultBuffer.Should().BeEquivalentTo(expectedBuffer);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(16)]
        [DataRow(SegmentSize)]
        [DataRow(98)]
        [DataRow(SegmentSize * 3)]
        [DataRow(478)]
        [DataRow(1024 * 5)]
        public void CopyTo_FromFastMemoryStreamToFastStreamMemory(int bufferSize)
        {
            // Arrange
            var buffer = this.fixture
                .CreateMany<byte>(bufferSize)
                .ToArray();

            var origin = new FastMemoryStream(this.managerMock.Object, SegmentSize);

            origin.Write(buffer, 0, buffer.Length);
            origin.Position = 0;

            // Act
            origin.CopyTo(this.target);

            // Assert
            this.target.Should().BeEquivalentTo(origin);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(16)]
        [DataRow(SegmentSize)]
        [DataRow(98)]
        [DataRow(SegmentSize * 3)]
        [DataRow(478)]
        [DataRow(1024 * 5)]
        public void CopyTo_WithDifferentSegmentSize(int bufferSize)
        {
            // Arrange
            var buffer = this.fixture
                .CreateMany<byte>(bufferSize)
                .ToArray();

            var origin = new FastMemoryStream(this.managerMock.Object, SegmentSize * 2);

            origin.Write(buffer, 0, buffer.Length);
            origin.Position = 0;

            // Act
            origin.CopyTo(this.target);

            // Assert
            this.target.Should().BeEquivalentTo(origin);
        }
    }
}
