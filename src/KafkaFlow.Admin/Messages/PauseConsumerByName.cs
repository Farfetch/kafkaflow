namespace KafkaFlow.Admin.Messages
{
    using System.Runtime.Serialization;

    [DataContract]
    public class PauseConsumerByName : IAdminMessage
    {
        [DataMember(Order = 1)]
        public string ConsumerName { get; set; }
    }
}
