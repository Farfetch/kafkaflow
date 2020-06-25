namespace KafkaFlow.Samples.Common
{
    using System.Runtime.Serialization;

    [DataContract]
    public class TestMessage
    {
        [DataMember(Order = 1)]
        public string Text { get; set; }
    }

    [DataContract]
    public class TestMessage2
    {
        [DataMember(Order = 1)]
        public string Value { get; set; }
    }
}
