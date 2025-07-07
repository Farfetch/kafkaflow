using System;

namespace KafkaFlow;

/// <summary>
/// Transfer interface for the DeliveryResult in Confluent.Kafka
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public interface IDeliveryResultFlow<out TKey, out TValue>
{
    string Topic { get; }

    int Partition { get; }

    long Offset { get; }

    object Status { get; }

    object Message { get; }

    TKey Key { get; }

    TValue Value { get; }

    DateTime Timestamp { get; }

    object Headers { get; }
}