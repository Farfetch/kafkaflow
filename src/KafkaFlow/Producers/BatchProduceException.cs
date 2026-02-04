using System;
using System.Collections.Generic;

namespace KafkaFlow.Producers;

/// <summary>
/// Exception thrown by <see cref="M:MessageProducer.BatchProduceAsync"/>
/// </summary>
public class BatchProduceException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BatchProduceException"/> class.
    /// </summary>
    /// <param name="items">The items contained in the batch</param>
    public BatchProduceException(IReadOnlyCollection<BatchProduceItem> items)
    {
        this.Items = items;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BatchProduceException"/> class.
    /// </summary>
    /// <param name="items">The items contained in the batch</param>
    /// <param name="innerException">The exception that caused the batch produce to fail</param>
    public BatchProduceException(IReadOnlyCollection<BatchProduceItem> items, Exception innerException)
        : base("One or more messages in the batch failed to produce. See InnerException for details.", innerException)
    {
        this.Items = items;
    }

    /// <summary>
    /// Gets the requested items to produce with <see cref="P:BatchProduceItem.DeliveryReport"/> filled
    /// </summary>
    public IReadOnlyCollection<BatchProduceItem> Items { get; }
}
