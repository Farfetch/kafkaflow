namespace KafkaFlow.Producers;

/// <summary>
/// Transfer object for the DeliveryReport in Confluent.Kafka
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public sealed class DeliveryReportFlow<TKey, TValue> : IDeliveryReportFlow<TKey, TValue>
{
    public string Topic { get; set; }
    public int Partition { get; set; }
    public long Offset { get; set; }
    public IError Error { get; set; }
}