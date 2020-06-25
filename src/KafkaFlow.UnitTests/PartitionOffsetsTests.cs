namespace KafkaFlow.UnitTests
{
    using System;
    using FluentAssertions;
    using KafkaFlow.Consumers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PartitionOffsetsTests
    {
        [TestMethod]
        public void InitializeLastOffset_InitializeTheValue_Success()
        {
            // Arrange
            var offsets = new PartitionOffsets();

            // Act
            offsets.InitializeLastOffset(1);

            // Assert
            offsets.LastOffset.Should().Be(1);
        }

        [TestMethod]
        public void InitializeLastOffset_TryInitializeAgain_ThrowsException()
        {
            // Arrange
            var offsets = new PartitionOffsets();
            offsets.InitializeLastOffset(1);

            // Act
            Action act = () => offsets.InitializeLastOffset(1);

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void ShouldUpdateOffset_WithoutInitialization_ThrowsException()
        {
            // Arrange
            var offsets = new PartitionOffsets();

            // Act
            Action act = () => offsets.ShouldUpdateOffset(1);

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void ShouldUpdateOffset_NextOffset_ShouldUpdate()
        {
            // Arrange
            var offsets = new PartitionOffsets();
            offsets.InitializeLastOffset(0);

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
            offsets.InitializeLastOffset(0);

            // Act
            var shouldUpdate = offsets.ShouldUpdateOffset(2);

            // Assert
            Assert.IsFalse(shouldUpdate);
            Assert.AreEqual(0, offsets.LastOffset);
        }


        [TestMethod]
        public void ShouldUpdateOffset_WithManyGaps_ShouldUpdate()
        {
            // Arrange
            var offsets = new PartitionOffsets();
            offsets.InitializeLastOffset(0);

            // Act
            var results = new[]
            {
                new
                {
                    ShouldUpdateResult = offsets.ShouldUpdateOffset(5),
                    ShouldUpdateExpected = false,
                    LastOffsetResult = offsets.LastOffset,
                    LastOffsetExpected = 0
                },
                new
                {
                    ShouldUpdateResult = offsets.ShouldUpdateOffset(1),
                    ShouldUpdateExpected = true,
                    LastOffsetResult = offsets.LastOffset,
                    LastOffsetExpected = 1
                },
                new
                {
                    ShouldUpdateResult = offsets.ShouldUpdateOffset(2),
                    ShouldUpdateExpected = true,
                    LastOffsetResult = offsets.LastOffset,
                    LastOffsetExpected = 2
                },
                new
                {
                    ShouldUpdateResult = offsets.ShouldUpdateOffset(8),
                    ShouldUpdateExpected = false,
                    LastOffsetResult = offsets.LastOffset,
                    LastOffsetExpected = 2
                },
                new
                {
                    ShouldUpdateResult = offsets.ShouldUpdateOffset(3),
                    ShouldUpdateExpected = true,
                    LastOffsetResult = offsets.LastOffset,
                    LastOffsetExpected = 3
                },
                new
                {
                    ShouldUpdateResult = offsets.ShouldUpdateOffset(6),
                    ShouldUpdateExpected = false,
                    LastOffsetResult = offsets.LastOffset,
                    LastOffsetExpected = 3
                },
                new
                {
                    ShouldUpdateResult = offsets.ShouldUpdateOffset(4),
                    ShouldUpdateExpected = true,
                    LastOffsetResult = offsets.LastOffset,
                    LastOffsetExpected = 6
                },
                new
                {
                    ShouldUpdateResult = offsets.ShouldUpdateOffset(7),
                    ShouldUpdateExpected = true,
                    LastOffsetResult = offsets.LastOffset,
                    LastOffsetExpected = 8
                },
                new
                {
                    ShouldUpdateResult = offsets.ShouldUpdateOffset(9),
                    ShouldUpdateExpected = true,
                    LastOffsetResult = offsets.LastOffset,
                    LastOffsetExpected = 9
                }
            };

            // Assert
            foreach (var result in results)
            {
                Assert.AreEqual(result.ShouldUpdateExpected, result.ShouldUpdateResult);
                Assert.AreEqual(result.LastOffsetExpected, result.LastOffsetResult);
            }
        }
    }
}
