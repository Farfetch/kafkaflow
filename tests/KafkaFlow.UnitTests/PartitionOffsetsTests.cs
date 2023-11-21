using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using KafkaFlow.Consumers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KafkaFlow.UnitTests
{
    [TestClass]
    public class PartitionOffsetsTests
    {
        [TestMethod]
        public void TryDequeue_WithoutEnqueuing_ThrowsException()
        {
            // Arrange
            var offsets = new PartitionOffsets();

            // Act
            Func<bool> act = () => offsets.TryDequeue(MockConsumerContext(1));

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void TryDequeue_WithSequencedContext_SetDequeuedContext()
        {
            // Arrange
            var offsets = new PartitionOffsets();

            var context = MockConsumerContext(1);

            offsets.Enqueue(context);

            // Act
            var isCommitAllowed = offsets.TryDequeue(context);

            // Assert
            Assert.IsTrue(isCommitAllowed);
            Assert.AreEqual(1, offsets.DequeuedContext.Offset);
        }

        [TestMethod]
        public void TryDequeue_WithNonSequencedContexts_UnsetDequeuedContext()
        {
            // Arrange
            var offsets = new PartitionOffsets();
            offsets.Enqueue(MockConsumerContext(1));

            // Act
            var isCommitAllowed = offsets.TryDequeue(MockConsumerContext(2));

            // Assert
            Assert.IsFalse(isCommitAllowed);
            Assert.IsNull(offsets.DequeuedContext);
        }

        [TestMethod]
        public void TryDequeue_WithUnorderedSequence_SetDequeuedContextWhenAllowed()
        {
            // Arrange
            var offsets = new PartitionOffsets();

            var context1 = MockConsumerContext(1);
            var context2 = MockConsumerContext(2);
            var context4 = MockConsumerContext(4);
            var context5 = MockConsumerContext(5);

            offsets.Enqueue(context1);
            offsets.Enqueue(context2);
            offsets.Enqueue(context4);
            offsets.Enqueue(context5);

            // Act
            var results = new[]
            {
                new
                {
                    Dequeued = offsets.TryDequeue(context5),
                    ExcpectedDequeuedResult = false,
                    DequeuedContextOffset = offsets.DequeuedContext?.Offset,
                    ExpectedDequeuedContextOffset = -1,
                },
                new
                {
                    Dequeued = offsets.TryDequeue(context1),
                    ExcpectedDequeuedResult = true,
                    DequeuedContextOffset = offsets.DequeuedContext?.Offset,
                    ExpectedDequeuedContextOffset = 1,
                },
                new
                {
                    Dequeued = offsets.TryDequeue(context2),
                    ExcpectedDequeuedResult = true,
                    DequeuedContextOffset = offsets.DequeuedContext?.Offset,
                    ExpectedDequeuedContextOffset = 2,
                },
                new
                {
                    Dequeued = offsets.TryDequeue(context4),
                    ExcpectedDequeuedResult = true,
                    DequeuedContextOffset = offsets.DequeuedContext?.Offset,
                    ExpectedDequeuedContextOffset = 5,
                },
            };

            // Assert
            foreach (var result in results)
            {
                Assert.AreEqual(result.ExcpectedDequeuedResult, result.Dequeued);
                Assert.AreEqual(result.ExpectedDequeuedContextOffset, result.DequeuedContextOffset ?? -1);
            }
        }

        [TestMethod]
        public void TryDequeue_WithConcurrentCalls_RemainTheLastDequeuedContext()
        {
            // Arrange
            const int count = 100;
            const int lastOffset = count - 1;

            var target = new PartitionOffsets();
            var contexts = new IConsumerContext[count];

            var taskCompletionSource = new TaskCompletionSource();

            for (var i = 0; i < count; i++)
            {
                contexts[i] = MockConsumerContext(i);
                target.Enqueue(contexts[i]);
            }

            // Act
            var tasks = new List<Task>();
            for (var i = 0; i < count; i++)
            {
                tasks.Add(
                    Task.Factory.StartNew(
                        async index =>
                        {
                            await taskCompletionSource.Task;

                            target.TryDequeue(contexts[(int)index]);
                        },
                        i));
            }

            taskCompletionSource.SetResult();

            Task.WaitAll(tasks.ToArray());

            // Assert
            Assert.AreEqual(lastOffset, target.DequeuedContext.Offset);
        }

        private static IConsumerContext MockConsumerContext(int offset)
        {
            var mock = new Mock<IConsumerContext>();

            mock
                .SetupGet(x => x.Offset)
                .Returns(offset);

            return mock.Object;
        }
    }
}
