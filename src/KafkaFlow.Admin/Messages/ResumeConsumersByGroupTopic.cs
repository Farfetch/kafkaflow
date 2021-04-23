namespace KafkaFlow.Admin.Messages
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The message that resume a paused topic
    /// </summary>
    [DataContract]
    public class ResumeConsumersByGroupTopic : IAdminMessage
    {
        /// <summary>
        /// Gets or sets consumer group id
        /// </summary>
        [DataMember(Order = 1)]
        public string GroupId { get; set; }

        /// <summary>
        /// Gets or sets the topic name
        /// </summary>
        [DataMember(Order = 2)]
        public string Topic { get; set; }
    }
}
