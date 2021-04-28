namespace KafkaFlow.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using KafkaFlow.Consumers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PartitionOffsetsTests
    {
        [TestMethod]
        public void AddOffset_InitializeTheValue_DoNothing()
        {
            // Arrange
            var offsets = new PartitionOffsets();

            // Act
            offsets.AddOffset(1);

            // Assert
            offsets.LastOffset.Should().Be(-1);
        }

        [TestMethod]
        public void ShouldUpdate_WithoutAddingOffsets_ThrowsException()
        {
            // Arrange
            var offsets = new PartitionOffsets();

            // Act
            Func<bool> act = () => offsets.ShouldUpdateOffset(1);

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void ShouldUpdateOffset_NextOffset_ShouldUpdate()
        {
            // Arrange
            var offsets = new PartitionOffsets();
            offsets.AddOffset(1);

            // Act
            var shouldUpdate = offsets.ShouldUpdateOffset(1);

            // Assert
            Assert.IsTrue(shouldUpdate);
            Assert.AreEqual(1, offsets.LastOffset);
        }

        [TestMethod]
        public void ShouldUpdateOffset_WithOneGap_ShouldNotUpdate()
        {
            // Arrange
            var offsets = new PartitionOffsets();
            offsets.AddOffset(1);

            // Act
            var shouldUpdate = offsets.ShouldUpdateOffset(2);

            // Assert
            Assert.IsFalse(shouldUpdate);
            Assert.AreEqual(-1, offsets.LastOffset);
        }

        [TestMethod]
        public void ShouldUpdateOffset_WithManyGaps_ShouldUpdate()
        {
            // Arrange
            var offsets = new PartitionOffsets();
            offsets.AddOffset(1);
            offsets.AddOffset(2);
            offsets.AddOffset(4);
            offsets.AddOffset(5);
            offsets.AddOffset(7);
            offsets.AddOffset(8);
            offsets.AddOffset(15);
            offsets.AddOffset(20);
            offsets.AddOffset(50);

            // Act
            var results = new[]
            {
                new
                {
                    ShouldUpdateResult = offsets.ShouldUpdateOffset(7),
                    ShouldUpdateExpected = false,
                    LastOffsetResult = offsets.LastOffset,
                    LastOffsetExpected = -1,
                },
                new
                {
                    ShouldUpdateResult = offsets.ShouldUpdateOffset(1),
                    ShouldUpdateExpected = true,
                    LastOffsetResult = offsets.LastOffset,
                    LastOffsetExpected = 1,
                },
                new
                {
                    ShouldUpdateResult = offsets.ShouldUpdateOffset(2),
                    ShouldUpdateExpected = true,
                    LastOffsetResult = offsets.LastOffset,
                    LastOffsetExpected = 2,
                },
                new
                {
                    ShouldUpdateResult = offsets.ShouldUpdateOffset(20),
                    ShouldUpdateExpected = false,
                    LastOffsetResult = offsets.LastOffset,
                    LastOffsetExpected = 2,
                },
                new
                {
                    ShouldUpdateResult = offsets.ShouldUpdateOffset(5),
                    ShouldUpdateExpected = false,
                    LastOffsetResult = offsets.LastOffset,
                    LastOffsetExpected = 2,
                },
                new
                {
                    ShouldUpdateResult = offsets.ShouldUpdateOffset(8),
                    ShouldUpdateExpected = false,
                    LastOffsetResult = offsets.LastOffset,
                    LastOffsetExpected = 2,
                },
                new
                {
                    ShouldUpdateResult = offsets.ShouldUpdateOffset(4),
                    ShouldUpdateExpected = true,
                    LastOffsetResult = offsets.LastOffset,
                    LastOffsetExpected = 8,
                },
                new
                {
                    ShouldUpdateResult = offsets.ShouldUpdateOffset(15),
                    ShouldUpdateExpected = true,
                    LastOffsetResult = offsets.LastOffset,
                    LastOffsetExpected = 20,
                },
                new
                {
                    ShouldUpdateResult = offsets.ShouldUpdateOffset(50),
                    ShouldUpdateExpected = true,
                    LastOffsetResult = offsets.LastOffset,
                    LastOffsetExpected = 50,
                },
            };

            // Assert
            foreach (var result in results)
            {
                Assert.AreEqual(result.ShouldUpdateExpected, result.ShouldUpdateResult);
                Assert.AreEqual(result.LastOffsetExpected, result.LastOffsetResult);
            }
        }

        [TestMethod]
        public void ShouldUpdateOffset_WithManyConcurrentOffsets_ShouldUpdate()
        {
            // Arrange
            const int count = 1000000;

            var target = new PartitionOffsets();
            var offsets = new List<int>(count);

            for (var i = 0; i < count; i += 2)
            {
                target.AddOffset(i);
                offsets.Add(i);
            }

            // Act
            Parallel.ForEach(offsets, offset => target.ShouldUpdateOffset(offset));

            // Assert
            Assert.AreEqual(999998, target.LastOffset);
        }
    }
}
