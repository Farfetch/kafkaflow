namespace KafkaFlow.Admin.Messages
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The message that resume a paused consumer
    /// </summary>
    [DataContract]
    public class ResumeConsumerByName : IAdminMessage
    {
        /// <summary>
        /// Gets or sets the consumer name
        /// </summary>
        [DataMember(Order = 1)]
        public string ConsumerName { get; set; }
    }
}
