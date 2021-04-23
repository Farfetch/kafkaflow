namespace KafkaFlow.Admin.Messages
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The message that rewind the offset of all partitions/topics of a consumer to the beginning
    /// </summary>
    [DataContract]
    public class ResetConsumerOffset : IAdminMessage
    {
        /// <summary>
        /// Gets or sets the consumer name
        /// </summary>
        [DataMember(Order = 1)]
        public string ConsumerName { get; set; }
    }
}
