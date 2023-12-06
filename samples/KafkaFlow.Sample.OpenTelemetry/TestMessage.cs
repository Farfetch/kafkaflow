using System.Runtime.Serialization;

namespace KafkaFlow.Sample.OpenTelemetry;

[DataContract]
public class TestMessage
{
    [DataMember(Order = 1)] 
    public string Text { get; set; }
}