namespace KafkaFlow.Producers;

/// <summary>
/// Transport Object for Error object in Confluent.Kafka
/// </summary>
public sealed class ErrorFlow : IError
{
    public object Code { get; set; }
    public bool IsError { get; set; }
    public string Reason { get; set; }
    public bool IsFatal { get; set; }
}