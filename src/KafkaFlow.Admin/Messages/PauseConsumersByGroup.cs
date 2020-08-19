namespace KafkaFlow.Admin.Messages
{
    using System.Runtime.Serialization;

    [DataContract]
    public class PauseConsumersByGroup : IAdminMessage
    {
        [DataMember(Order = 1)]
        public string GroupId { get; set; }
    }
}
