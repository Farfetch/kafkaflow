using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Confluent.Kafka;

namespace KafkaFlow.Producers;

/// <summary>
/// Manages the state for batch produce operations, coordinating message collection
/// across parallel middleware executions to preserve message ordering.
/// </summary>
internal sealed class BatchProduceContext
{
    private readonly ConcurrentBag<BatchMessageEntry> _entries = new();

    public IReadOnlyList<BatchMessageEntry> GetEntries() =>
        _entries.OrderBy(x => x.BatchIndex).ToList();

    public IReadOnlyList<BatchMessageEntry> GetSuccessfulEntries() =>
        _entries.Where(e => e.IsSuccess).OrderBy(e => e.BatchIndex).ToList();

    public IReadOnlyList<BatchMessageEntry> GetFailedEntries() =>
        _entries.Where(e => !e.IsSuccess).OrderBy(e => e.BatchIndex).ToList();

    /// <summary>
    /// Registers a message that completed middleware processing successfully.
    /// </summary>
    public void Register(
        int batchIndex,
        BatchProduceItem item,
        Message<byte[], byte[]> messageToProduce,
        IMessageContext context)
    {
        _entries.Add(new BatchMessageEntry(batchIndex, item, true, messageToProduce, context, null));
    }

    /// <summary>
    /// Registers that a middleware chain failed. Sets the DeliveryReport on the item directly.
    /// </summary>
    public void RegisterFailure(int batchIndex, BatchProduceItem item, Exception exception)
    {
        item.DeliveryReport = new DeliveryReport<byte[], byte[]>
        {
            Topic = item.Topic,
            Error = new Error(ErrorCode.Local_Fail, exception.Message),
            Status = PersistenceStatus.NotPersisted,
        };
        _entries.Add(new BatchMessageEntry(batchIndex, item, false, null, null, exception));
    }
}

/// <summary>
/// Represents a message entry collected during batch processing.
/// </summary>
internal record struct BatchMessageEntry(
    int BatchIndex,
    BatchProduceItem Item,
    bool IsSuccess,
    Message<byte[], byte[]> MessageToProduce,
    IMessageContext Context,
    Exception Error);
