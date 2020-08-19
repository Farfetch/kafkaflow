namespace KafkaFlow.Admin.Messages
{
    using System.Runtime.Serialization;

    [DataContract]
    public class ResetConsumerOffset : IAdminMessage
    {
        [DataMember(Order = 1)]
        public string ConsumerName { get; set; }
    }
}
