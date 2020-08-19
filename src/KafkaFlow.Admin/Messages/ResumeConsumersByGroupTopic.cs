namespace KafkaFlow.Admin.Messages
{
    using System.Runtime.Serialization;

    [DataContract]
    public class ResumeConsumersByGroupTopic : IAdminMessage
    {
        [DataMember(Order = 1)]
        public string GroupId { get; set; }

        [DataMember(Order = 2)]
        public string Topic { get; set; }
    }
}
