namespace KafkaFlow.Admin.Messages
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The message that resume a paused consumer group
    /// </summary>
    [DataContract]
    public class ResumeConsumersByGroup : IAdminMessage
    {
        /// <summary>
        /// Gets or sets the consumer group id
        /// </summary>
        [DataMember(Order = 1)]
        public string GroupId { get; set; }
    }
}
