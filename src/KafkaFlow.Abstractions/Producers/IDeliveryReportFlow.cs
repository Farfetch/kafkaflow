namespace KafkaFlow;

/// <summary>
/// Transfer interface for the DeliveryReport in Confluent.Kafka
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public interface IDeliveryReportFlow<out TKey, out TValue> : IDeliveryResultFlow<TKey, TValue>
{
    string Topic { get; }

    int Partition { get; }

    long Offset { get; }

    IError Error { get; }

    /// <summary>
    /// Unused
    /// </summary>
    TKey Key { get; }

    /// <summary>
    /// Unused
    /// </summary>
    TValue Value { get; }
}