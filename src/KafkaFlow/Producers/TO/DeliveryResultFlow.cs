using System;

namespace KafkaFlow.Producers;

/// <summary>
/// Transfer object for the DeliveryResult in Confluent.Kafka
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public sealed class DeliveryResultFlow<TKey, TValue> : IDeliveryResultFlow<TKey, TValue>
{
    public string Topic { get; set; }
    public int Partition { get; set; }
    public long Offset { get; set; }
    public object Status { get; set; }
    public object Message { get; set; }
    public TKey Key { get; set; }
    public TValue Value { get; set; }
    public DateTime Timestamp { get; set; }
    public object Headers { get; set; }
}
