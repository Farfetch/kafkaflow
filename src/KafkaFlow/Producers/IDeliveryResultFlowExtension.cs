using System;
using Confluent.Kafka;

namespace KafkaFlow.Producers;

/// <summary>
/// No needed
/// </summary>
public static class IDeliveryResultFlowExtension
{
    /// <summary>
    /// Converts a Confluent.Kafka delivery result to a KafkaFlow delivery result.
    /// </summary>
    /// <param name="deliveryResult"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static DeliveryResult<TKey, TValue> ToDeliveryResult<TKey, TValue>(
        this IDeliveryResultFlow<TKey, TValue> deliveryResult)
    {
        if (deliveryResult is null)
        {
            throw new ArgumentNullException(nameof(deliveryResult));
        }

        return new DeliveryResult<TKey, TValue>
        {
            Topic = deliveryResult.Topic,
            Partition = deliveryResult.Partition,
            Offset = deliveryResult.Offset,
            Status = (PersistenceStatus)deliveryResult.Status,
            Message = (Message<TKey, TValue>)deliveryResult.Message,
            Key = deliveryResult.Key,
            Value = deliveryResult.Value,
            Timestamp = new Timestamp(deliveryResult.Timestamp),
            Headers = (Headers)deliveryResult.Headers,
        };
    }
}