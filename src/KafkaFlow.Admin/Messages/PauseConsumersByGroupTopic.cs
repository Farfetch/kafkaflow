namespace KafkaFlow.Admin.Messages
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The message that pauses a specific topic
    /// </summary>
    [DataContract]
    public class PauseConsumersByGroupTopic : IAdminMessage
    {
        /// <summary>
        /// Gets or sets the consumer group id
        /// </summary>
        [DataMember(Order = 1)]
        public string GroupId { get; set; }

        /// <summary>
        /// Gets or sets the consumer topic
        /// </summary>
        [DataMember(Order = 2)]
        public string Topic { get; set; }
    }
}
