using Confluent.Kafka;
using KafkaFlow.Producers;

namespace KafkaFlow;

/// <summary>
/// No needed
/// </summary>
public static class DeliveryReportExtension
{
    /// <summary>
    /// Converts a Confluent.Kafka delivery report to a KafkaFlow delivery report.
    /// </summary>
    /// <param name="report"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static IDeliveryReportFlow<TKey, TValue> ToIDeliveryReportFlow<TKey, TValue>(this DeliveryReport<TKey, TValue> report)
    {
        return new DeliveryReportFlow<TKey, TValue>
        {
            Topic = report.Topic,
            Partition = report.Partition,
            Offset = report.Offset,
            Error = report.Error.ToIError(),
        };
    }
}