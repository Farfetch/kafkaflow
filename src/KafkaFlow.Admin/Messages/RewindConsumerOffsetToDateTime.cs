namespace KafkaFlow.Admin.Messages
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The message that rewind a consumer to a point in time
    /// </summary>
    [DataContract]
    public class RewindConsumerOffsetToDateTime : IAdminMessage
    {
        /// <summary>
        /// Gets or sets the consumer name
        /// </summary>
        [DataMember(Order = 1)]
        public string ConsumerName { get; set; }

        /// <summary>
        /// Gets or sets the point in time
        /// </summary>
        [DataMember(Order = 2)]
        public DateTime DateTime { get; set; }
    }
}
