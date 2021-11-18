namespace KafkaFlow.Admin.Messages
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The message that pauses an entire consumer group
    /// </summary>
    [DataContract]
    public class PauseConsumersByGroup : IAdminMessage
    {
        /// <summary>
        /// Gets or sets the consumer group id
        /// </summary>
        [DataMember(Order = 1)]
        public string GroupId { get; set; }

        /// <summary>
        /// Gets or sets the topics that will be paused
        /// </summary>
        [DataMember(Order = 2)]
        public IList<string> Topics { get; set; } = new List<string>();
    }
}
