using System.Runtime.Serialization;

namespace KafkaFlow.Sample;

[DataContract]
public class TestMessage
{
    [DataMember(Order = 1)] 
    public string Text { get; set; }
}