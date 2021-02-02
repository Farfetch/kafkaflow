namespace KafkaFlow.Producers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;

    /// <summary>
    /// </summary>
    public static class BatchProduceExtension
    {
        /// <summary>
        /// Calls the Produce() method in loop for high throughput scenarios
        /// </summary>
        /// <param name="producer"></param>
        /// <param name="items">All messages to produce</param>
        /// <param name="throwIfAnyProduceFail">indicates if the method should throw a <see cref="BatchProduceException"/> if any message fail</param>
        /// <returns>A Task that will be marked as completed when all produce operations end</returns>
        public static Task<IReadOnlyCollection<BatchProduceItem>> BatchProduceAsync(
            this IMessageProducer producer,
            IReadOnlyCollection<BatchProduceItem> items,
            bool throwIfAnyProduceFail = true)
        {
            var completionSource = new TaskCompletionSource<IReadOnlyCollection<BatchProduceItem>>();

            var pendingProduceCount = items.Count;
            var hasErrors = false;

            if (pendingProduceCount == 0)
            {
                completionSource.SetResult(items);
            }

            foreach (var item in items)
            {
                producer.Produce(
                    item.Topic,
                    item.PartitionKey,
                    item.Message,
                    item.Headers,
                    report =>
                    {
                        item.DeliveryReport = report;

                        if (report.Error.IsError)
                        {
                            hasErrors = true;
                        }

                        if (Interlocked.Decrement(ref pendingProduceCount) != 0)
                        {
                            return;
                        }

                        if (throwIfAnyProduceFail && hasErrors)
                        {
                            completionSource.SetException(new BatchProduceException(items));
                        }
                        else
                        {
                            completionSource.SetResult(items);
                        }
                    });
            }

            return completionSource.Task;
        }
    }

    /// <summary>
    /// Represents a message to be produced in batch
    /// </summary>
    public class BatchProduceItem
    {
        /// <summary>
        /// The message topic name
        /// </summary>
        public string Topic { get; }

        /// <summary>
        /// The message partition key
        /// </summary>
        public string PartitionKey { get; }

        /// <summary>
        /// The message object
        /// </summary>
        public object Message { get; }

        /// <summary>
        /// The message headers
        /// </summary>
        public IMessageHeaders Headers { get; }

        /// <summary>
        /// The delivery report after the production
        /// </summary>
        public DeliveryReport<byte[], byte[]> DeliveryReport { get; internal set; }

        /// <summary>
        /// Creates a batch produce item
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="partitionKey"></param>
        /// <param name="message"></param>
        /// <param name="headers"></param>
        public BatchProduceItem(
            string topic,
            string partitionKey,
            object message,
            IMessageHeaders headers)
        {
            this.Topic = topic;
            this.PartitionKey = partitionKey;
            this.Message = message;
            this.Headers = headers;
        }
    }

    public class BatchProduceException : Exception
    {
        public IReadOnlyCollection<BatchProduceItem> Items { get; }

        public BatchProduceException(IReadOnlyCollection<BatchProduceItem> items)
        {
            this.Items = items;
        }
    }
}
