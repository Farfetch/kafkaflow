namespace KafkaFlow.UnitTests
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using KafkaFlow.Consumers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PartitionOffsetsTests
    {
        [TestMethod]
        public void ShouldUpdate_WithoutAddingOffsets_ThrowsException()
        {
            // Arrange
            var offsets = new PartitionOffsets();

            // Act
            Func<bool> act = () => offsets.ShouldCommit(1, out _);

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void ShouldUpdateOffset_NextOffset_ShouldUpdate()
        {
            // Arrange
            var offsets = new PartitionOffsets();
            offsets.Enqueue(1);

            // Act
            var shouldUpdate = offsets.ShouldCommit(1, out var offset);

            // Assert
            Assert.IsTrue(shouldUpdate);
            Assert.AreEqual(1, offset);
        }

        [TestMethod]
        public void ShouldUpdateOffset_WithOneGap_ShouldNotUpdate()
        {
            // Arrange
            var offsets = new PartitionOffsets();
            offsets.Enqueue(1);

            // Act
            var shouldUpdate = offsets.ShouldCommit(2, out var offset);

            // Assert
            Assert.IsFalse(shouldUpdate);
            Assert.AreEqual(-1, offset);
        }

        [TestMethod]
        public void ShouldUpdateOffset_WithManyGaps_ShouldUpdate()
        {
            // Arrange
            var offsets = new PartitionOffsets();
            offsets.Enqueue(1);
            offsets.Enqueue(2);
            offsets.Enqueue(4);
            offsets.Enqueue(5);
            offsets.Enqueue(7);
            offsets.Enqueue(8);
            offsets.Enqueue(15);
            offsets.Enqueue(20);
            offsets.Enqueue(50);

            // Act
            var results = new[]
            {
                new
                {
                    ShouldUpdateResult = offsets.ShouldCommit(7, out long offset),
                    ShouldUpdateExpected = false,
                    LastOffsetResult = offset,
                    LastOffsetExpected = -1,
                },
                new
                {
                    ShouldUpdateResult = offsets.ShouldCommit(1, out offset),
                    ShouldUpdateExpected = true,
                    LastOffsetResult = offset,
                    LastOffsetExpected = 1,
                },
                new
                {
                    ShouldUpdateResult = offsets.ShouldCommit(2, out offset),
                    ShouldUpdateExpected = true,
                    LastOffsetResult = offset,
                    LastOffsetExpected = 2,
                },
                new
                {
                    ShouldUpdateResult = offsets.ShouldCommit(20, out offset),
                    ShouldUpdateExpected = false,
                    LastOffsetResult = offset,
                    LastOffsetExpected = -1,
                },
                new
                {
                    ShouldUpdateResult = offsets.ShouldCommit(5, out offset),
                    ShouldUpdateExpected = false,
                    LastOffsetResult = offset,
                    LastOffsetExpected = -1,
                },
                new
                {
                    ShouldUpdateResult = offsets.ShouldCommit(8, out offset),
                    ShouldUpdateExpected = false,
                    LastOffsetResult = offset,
                    LastOffsetExpected = -1,
                },
                new
                {
                    ShouldUpdateResult = offsets.ShouldCommit(4, out offset),
                    ShouldUpdateExpected = true,
                    LastOffsetResult = offset,
                    LastOffsetExpected = 8,
                },
                new
                {
                    ShouldUpdateResult = offsets.ShouldCommit(15, out offset),
                    ShouldUpdateExpected = true,
                    LastOffsetResult = offset,
                    LastOffsetExpected = 20,
                },
                new
                {
                    ShouldUpdateResult = offsets.ShouldCommit(50, out offset),
                    ShouldUpdateExpected = true,
                    LastOffsetResult = offset,
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
            const int count = 1_000;

            var target = new PartitionOffsets();
            var offsetsCommitted = new ConcurrentBag<long>();

            var waitHandle = new ManualResetEvent(false);

            for (var i = 0; i < count; i += 2)
            {
                target.Enqueue(i);
            }

            // Act
            var tasks = new List<Task>();
            for (var i = 0; i < count; i += 2)
            {
                var offset = i;
                tasks.Add(
                    Task.Run(
                        () =>
                        {
                            waitHandle.WaitOne();

                            if (target.ShouldCommit(offset, out var lastProcessedOffset))
                            {
                                offsetsCommitted.Add(lastProcessedOffset);
                            }
                        }));
            }

            waitHandle.Set();

            Task.WaitAll(tasks.ToArray());

            // Assert
            Assert.AreEqual(count - 2, offsetsCommitted.Max());
        }
    }
}
