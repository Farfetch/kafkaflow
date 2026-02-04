using System;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using AwesomeAssertions;
using KafkaFlow.Producers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KafkaFlow.UnitTests.BatchProduce;

[TestClass]
public class BatchProduceContextTests
{
    [TestMethod]
    public void Register_ShouldStoreSuccessfulEntry()
    {
        // Arrange
        var context = new BatchProduceContext();
        var item = CreateBatchProduceItem("test-topic");
        var message = CreateMessage();
        var messageContext = Mock.Of<IMessageContext>();

        // Act
        context.Register(0, item, message, messageContext);
        var entries = context.GetEntries();

        // Assert
        entries.Should().HaveCount(1);
        entries[0].BatchIndex.Should().Be(0);
        entries[0].Item.Should().BeSameAs(item);
        entries[0].IsSuccess.Should().BeTrue();
        entries[0].MessageToProduce.Should().BeSameAs(message);
        entries[0].Context.Should().BeSameAs(messageContext);
        entries[0].Error.Should().BeNull();
    }

    [TestMethod]
    public void RegisterFailure_ShouldStoreFailedEntryAndSetDeliveryReport()
    {
        // Arrange
        var context = new BatchProduceContext();
        var item = CreateBatchProduceItem("test-topic");
        var exception = new InvalidOperationException("Test error");

        // Act
        context.RegisterFailure(1, item, exception);
        var entries = context.GetEntries();

        // Assert
        entries.Should().HaveCount(1);
        entries[0].BatchIndex.Should().Be(1);
        entries[0].Item.Should().BeSameAs(item);
        entries[0].IsSuccess.Should().BeFalse();
        entries[0].MessageToProduce.Should().BeNull();
        entries[0].Context.Should().BeNull();
        entries[0].Error.Should().BeSameAs(exception);

        // Verify DeliveryReport was set on the item
        item.DeliveryReport.Should().NotBeNull();
        item.DeliveryReport.Topic.Should().Be("test-topic");
        item.DeliveryReport.Error.Code.Should().Be(ErrorCode.Local_Fail);
        item.DeliveryReport.Error.Reason.Should().Be("Test error");
        item.DeliveryReport.Status.Should().Be(PersistenceStatus.NotPersisted);
    }

    [TestMethod]
    public void GetEntries_ShouldReturnAllEntriesSortedByBatchIndex()
    {
        // Arrange
        var context = new BatchProduceContext();
        var item = CreateBatchProduceItem("test-topic");
        var message = CreateMessage();
        var messageContext = Mock.Of<IMessageContext>();

        // Register in random order
        context.Register(2, item, message, messageContext);
        context.RegisterFailure(1, item, new Exception("Error"));
        context.Register(0, item, message, messageContext);

        // Act
        var entries = context.GetEntries();

        // Assert
        entries.Should().HaveCount(3);
        entries[0].BatchIndex.Should().Be(0);
        entries[1].BatchIndex.Should().Be(1);
        entries[2].BatchIndex.Should().Be(2);
    }

    [TestMethod]
    public void Register_ShouldBeThreadSafe()
    {
        // Arrange
        var context = new BatchProduceContext();
        var item = CreateBatchProduceItem("test-topic");
        var message = CreateMessage();
        var messageContext = Mock.Of<IMessageContext>();

        // Act - register from multiple threads in parallel
        var tasks = Enumerable.Range(0, 100)
            .Select(i => Task.Run(() => context.Register(i, item, message, messageContext)))
            .ToArray();

        Task.WaitAll(tasks);

        // Assert
        var entries = context.GetEntries();
        entries.Should().HaveCount(100);
        entries.Select(e => e.BatchIndex).Should().BeEquivalentTo(Enumerable.Range(0, 100));
    }

    [TestMethod]
    public void RegisterFailure_ShouldBeThreadSafe()
    {
        // Arrange
        var context = new BatchProduceContext();
        var exceptions = Enumerable.Range(0, 100)
            .Select(i => new InvalidOperationException($"Error {i}"))
            .ToArray();

        // Act - register failures from multiple threads in parallel
        var tasks = Enumerable.Range(0, 100)
            .Select(i => Task.Run(() =>
            {
                var item = CreateBatchProduceItem($"topic-{i}");
                context.RegisterFailure(i, item, exceptions[i]);
            }))
            .ToArray();

        Task.WaitAll(tasks);

        // Assert
        var entries = context.GetEntries().Where(e => !e.IsSuccess).ToList();
        entries.Should().HaveCount(100);
        entries.Select(e => e.BatchIndex).Should().BeEquivalentTo(Enumerable.Range(0, 100));
    }

    [TestMethod]
    public void MixedOperations_ShouldBeThreadSafe()
    {
        // Arrange
        var context = new BatchProduceContext();
        var message = CreateMessage();
        var messageContext = Mock.Of<IMessageContext>();

        // Act - mix successful and failed registrations from multiple threads
        var tasks = Enumerable.Range(0, 100)
            .Select(i => Task.Run(() =>
            {
                var item = CreateBatchProduceItem($"topic-{i}");
                if (i % 2 == 0)
                {
                    context.Register(i, item, message, messageContext);
                }
                else
                {
                    context.RegisterFailure(i, item, new Exception($"Error {i}"));
                }
            }))
            .ToArray();

        Task.WaitAll(tasks);

        // Assert
        var allEntries = context.GetEntries();
        var successful = allEntries.Where(e => e.IsSuccess).ToList();
        var failed = allEntries.Where(e => !e.IsSuccess).ToList();

        allEntries.Should().HaveCount(100);
        successful.Should().HaveCount(50);
        failed.Should().HaveCount(50);
    }

    private static Message<byte[], byte[]> CreateMessage()
    {
        return new Message<byte[], byte[]>
        {
            Key = new byte[] { 1, 2, 3 },
            Value = new byte[] { 4, 5, 6 },
        };
    }

    private static BatchProduceItem CreateBatchProduceItem(string topic, int? partition = null)
    {
        if (partition.HasValue)
        {
            return new BatchProduceItem(
                topic,
                messageKey: "test-key",
                messageValue: "test-value",
                headers: null,
                partition: partition.Value);
        }

        return new BatchProduceItem(
            topic,
            messageKey: "test-key",
            messageValue: "test-value",
            headers: null);
    }
}
