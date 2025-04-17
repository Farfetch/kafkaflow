using System;
using Confluent.Kafka;

namespace KafkaFlow.Producers;

/// <summary>
/// No needed
/// </summary>
public static class DeliveryResultExtension
{
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
