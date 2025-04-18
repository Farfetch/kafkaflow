namespace KafkaFlow;

/// <summary>
/// Transfer interface for the Error in Confluent.Kafka
/// </summary>
public interface IError
{
    public bool IsError { get; }

    public string Reason { get; }

    public bool IsFatal { get; }

    public object Code { get; }
}
