namespace KafkaFlow.Admin.Messages
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class RewindConsumerOffsetToDateTime : IAdminMessage
    {
        [DataMember(Order = 1)]
        public string ConsumerName { get; set; }

        [DataMember(Order = 2)]
        public DateTime DateTime { get; set; }
    }
}
