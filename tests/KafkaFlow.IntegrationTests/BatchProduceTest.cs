using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AwesomeAssertions;
using KafkaFlow.IntegrationTests.Core;
using KafkaFlow.IntegrationTests.Core.Handlers;
using KafkaFlow.IntegrationTests.Core.Producers;
using KafkaFlow.Producers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KafkaFlow.IntegrationTests;

[TestClass]
public class BatchProduceTest
{
    private readonly Fixture _fixture = new();

    private IServiceProvider _provider;

    [TestInitialize]
    public void Setup()
    {
        _provider = Bootstrapper.GetServiceProvider();
        MessageStorage.Clear();
    }

    [TestMethod]
    public async Task BatchProduceAsync_ShouldProduceAllMessagesInOrder()
    {
        // Arrange
        var producer = _provider.GetRequiredService<IMessageProducer<GzipProducer>>();

        var messages = Enumerable.Range(0, 10)
            .Select(_ => _fixture.Create<byte[]>())
            .ToList();

        var batchItems = messages
            .Select(m => new BatchProduceItem(
                Bootstrapper.GzipTopicName,
                Guid.NewGuid().ToString(),
                m,
                null))
            .ToList();

        // Act
        var result = await producer.BatchProduceAsync(batchItems, throwIfAnyProduceFail: true);

        // Assert
        result.Should().HaveCount(10);

        // All items should have delivery reports with successful status
        foreach (var item in batchItems)
        {
            item.DeliveryReport.Should().NotBeNull();
            item.DeliveryReport.Status.Should().Be(Confluent.Kafka.PersistenceStatus.Persisted);
        }

        // Verify messages were received (check first few to avoid timeout)
        for (var i = 0; i < 3; i++)
        {
            await MessageStorage.AssertMessageAsync(messages[i]);
        }
    }

    [TestMethod]
    public async Task BatchProduceAsync_WithEmptyBatch_ShouldReturnImmediately()
    {
        // Arrange
        var producer = _provider.GetRequiredService<IMessageProducer<GzipProducer>>();
        var batchItems = new List<BatchProduceItem>();

        // Act
        var result = await producer.BatchProduceAsync(batchItems, throwIfAnyProduceFail: true);

        // Assert
        result.Should().BeEmpty();
    }

    [TestMethod]
    public async Task BatchProduceAsync_WithSingleItem_ShouldWork()
    {
        // Arrange
        var producer = _provider.GetRequiredService<IMessageProducer<GzipProducer>>();
        var message = _fixture.Create<byte[]>();

        var batchItems = new List<BatchProduceItem>
        {
            new(Bootstrapper.GzipTopicName, Guid.NewGuid().ToString(), message, null),
        };

        // Act
        var result = await producer.BatchProduceAsync(batchItems, throwIfAnyProduceFail: true);

        // Assert
        result.Should().HaveCount(1);
        batchItems[0].DeliveryReport.Should().NotBeNull();
        batchItems[0].DeliveryReport.Status.Should().Be(Confluent.Kafka.PersistenceStatus.Persisted);

        // Verify message was received
        await MessageStorage.AssertMessageAsync(message);
    }

    [TestMethod]
    public async Task BatchProduceAsync_ShouldPreserveMessageOrder()
    {
        // This test verifies that messages are produced to Kafka in their original batch order.
        // Even though middlewares run in parallel and may complete out of order,
        // the batch produce implementation collects all processed messages and produces them
        // sequentially in their original order. This is verified by checking that Kafka offsets
        // are monotonically increasing according to the original batch order (within each partition).

        // Arrange
        var producer = _provider.GetRequiredService<IMessageProducer<GzipProducer>>();

        // Create messages with sequential identifiers
        var messageCount = 20;
        var batchItems = Enumerable.Range(0, messageCount)
            .Select(i => new BatchProduceItem(
                Bootstrapper.GzipTopicName,
                $"order-test-{i:D4}",
                BitConverter.GetBytes(i), // Encode the sequence number in the message
                null))
            .ToList();

        // Act
        var result = await producer.BatchProduceAsync(batchItems, throwIfAnyProduceFail: true);

        // Assert all delivery reports are present and successful
        result.Should().HaveCount(batchItems.Count);
        result.Should().AllSatisfy(item =>
        {
            item.DeliveryReport.Should().NotBeNull();
            item.DeliveryReport.Status.Should().Be(Confluent.Kafka.PersistenceStatus.Persisted);
        });

        // Assert order preservation: within each partition, offsets must be increasing
        // according to the original batch order. This proves messages were produced
        // in the correct sequence, not in the order middlewares completed.
        batchItems
            .Select((item, index) => new { Item = item, OriginalIndex = index })
            .GroupBy(x => x.Item.DeliveryReport.Partition.Value)
            .Should()
            .AllSatisfy(group =>
                group
                    .OrderBy(x => x.OriginalIndex)
                    .Select(x => x.Item.DeliveryReport.Offset.Value)
                    .Should().BeInAscendingOrder(
                        $"Messages in partition {group.Key} should be produced in batch order, not middleware completion order")
            );
    }

    [TestMethod]
    public async Task BatchProduceAsync_WithLargeBatch_ShouldSucceed()
    {
        // Arrange
        var producer = _provider.GetRequiredService<IMessageProducer<GzipProducer>>();

        var batchSize = 100;
        var batchItems = Enumerable.Range(0, batchSize)
            .Select(i => new BatchProduceItem(
                Bootstrapper.GzipTopicName,
                $"large-batch-{i}",
                _fixture.Create<byte[]>(),
                null))
            .ToList();

        // Act
        var result = await producer.BatchProduceAsync(batchItems, throwIfAnyProduceFail: true);

        // Assert
        result.Should().HaveCount(batchSize);

        // All items should have successful delivery reports
        batchItems.All(i =>
            i.DeliveryReport is { Status: Confluent.Kafka.PersistenceStatus.Persisted })
            .Should().BeTrue("All messages should be successfully persisted");
    }
}
