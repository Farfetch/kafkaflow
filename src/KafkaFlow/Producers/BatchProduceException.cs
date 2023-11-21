using System;
using System.Collections.Generic;

namespace KafkaFlow.Producers
{
    /// <summary>
    /// Exception thrown by <see cref="M:BatchProduceExtension.BatchProduceAsync"/>
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
        /// Gets the requested items to produce with <see cref="P:BatchProduceItem.DeliveryReport"/> filled
        /// </summary>
        public IReadOnlyCollection<BatchProduceItem> Items { get; }
    }
}
