namespace KafkaFlow.Client.Tests
{
    using KafkaFlow.Client.Protocol;
    using KafkaFlow.Client.Protocol.Streams;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class Crc32CHashTests
    {
        [TestMethod]
        public void Compute_EmptyArray()
        {
            // Arrange
            var data = new byte[0];
            const uint expected = 0;

            // Act
            var hash = Crc32CHash.Compute(data);

            // Assert
            Assert.AreEqual(expected, hash);
        }

        [TestMethod]
        public void Compute_ArrayWithZero()
        {
            // Arrange
            var data = new byte[] { 0 };
            const uint expected = 1383945041;

            // Act
            var hash = Crc32CHash.Compute(data);

            // Assert
            Assert.AreEqual(expected, hash);
        }

        [TestMethod]
        public void Compute_SmallArray()
        {
            // Arrange
            var data = new byte[] { 0, 11, 25, 78, 255, 100 };
            const uint expected = 857855865;

            // Act
            var hash = Crc32CHash.Compute(data);

            // Assert
            Assert.AreEqual(expected, hash);
        }

        [TestMethod]
        public void Compute_BigArray()
        {
            // Arrange
            var data = new byte[] { 254, 124, 147, 178, 123, 168, 14, 19, 85, 75, 96, 45, 35, 45, 82, 12, 21, 32, 47, 54 };
            const uint expected = 1921932177;

            // Act
            var hash = Crc32CHash.Compute(data);

            // Assert
            Assert.AreEqual(expected, hash);
        }

        [TestMethod]
        public void Compute_WithPartial_BigArray()
        {
            // Arrange
            var data1 = new byte[] { 254, 124, 147, 178, 123, 168, 14 };
            var data2 = new byte[] { 19, 85, 75, 96, 45, 35, 45, 82, 12, 21, 32, 47, 54 };

            const uint expected = 1921932177;

            // Act
            var hash = Crc32CHash.Compute(data1);
            hash = Crc32CHash.Compute(hash, data2);

            // Assert
            Assert.AreEqual(expected, hash);
        }

        [TestMethod]
        [DataRow(8)]
        [DataRow(1024)]
        public void Compute_MemoryWriter(int segmentSize)
        {
            // Arrange
            var data1 = new byte[] { 0, 0, 0, 254, 124, 147, 178, 123, 168, 14 };
            var data2 = new byte[] { 19, 85, 75, 96, 45, 35, 45, 82, 12, 21, 32, 47, 54 };

            var writer = new MemoryWriter(segmentSize);

            writer.Write(data1);
            writer.Write(data2);

            const uint expected = 1921932177;

            // Act
            var hash = writer.ComputeCRC32C(3, 20);

            // Assert
            Assert.AreEqual(expected, hash);
        }
    }
}
