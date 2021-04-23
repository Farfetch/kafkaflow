namespace KafkaFlow.Admin.Messages
{
    using System.Runtime.Serialization;

    /// <summary>
    /// A message that pauses a consumer
    /// </summary>
    [DataContract]
    public class PauseConsumerByName : IAdminMessage
    {
        /// <summary>
        /// Gets or sets the consumer name that will be paused
        /// </summary>
        [DataMember(Order = 1)]
        public string ConsumerName { get; set; }
    }
}
