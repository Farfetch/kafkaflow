using System;
using Confluent.Kafka;

namespace KafkaFlow.Producers;

/// <summary>
/// Converts between KafkaFlow and Confluent.Kafka delivery results.
/// </summary>
public static class DeliveryResultExtension
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

    /// <summary>
    /// Converts a KafkaFlow delivery result to a Confluent.Kafka delivery result.
    /// </summary>
    /// <param name="deliveryResult"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static IDeliveryResultFlow<TKey, TValue> ToIDeliveryResultFlow<TKey, TValue>(
        this DeliveryResult<TKey, TValue> deliveryResult)
    {
        return new DeliveryResultFlow<TKey, TValue>
        {
            Topic = deliveryResult.Topic,
            Partition = deliveryResult.Partition,
            Offset = deliveryResult.Offset,
            Status = deliveryResult.Status,
            Message = deliveryResult.Message,
            Key = deliveryResult.Key,
            Value = deliveryResult.Value,
            Timestamp = deliveryResult.Timestamp.UtcDateTime,
            Headers = deliveryResult.Headers,
        };
    }
}
