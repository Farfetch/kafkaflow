namespace KafkaFlow.Sample.Auth;

using System.Runtime.Serialization;

[DataContract]
public class TestMessage
{
    [DataMember(Order = 1)] 
    public string? Text { get; set; }
}