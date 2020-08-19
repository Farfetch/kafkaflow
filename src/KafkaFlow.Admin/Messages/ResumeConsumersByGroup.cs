namespace KafkaFlow.Admin.Messages
{
    using System.Runtime.Serialization;

    [DataContract]
    public class ResumeConsumersByGroup : IAdminMessage
    {
        [DataMember(Order = 1)]
        public string GroupId { get; set; }
    }
}
