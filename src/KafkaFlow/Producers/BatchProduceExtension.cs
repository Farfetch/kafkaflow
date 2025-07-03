using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaFlow.Producers;

/// <summary>
/// No needed
/// </summary>
public static class BatchProduceExtension
{
    /// <summary>
    /// Calls the Produce() method in loop for high throughput scenarios
    /// </summary>
    /// <param name="producer">The producer instance</param>
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
                item.MessageKey,
                item.MessageValue,
                item.Headers,
                deliveryReportFlow =>
                {
                    item.DeliveryReport = deliveryReportFlow.ToDeliveryReport();

                    if (deliveryReportFlow.Error.IsError)
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
