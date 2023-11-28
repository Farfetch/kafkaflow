using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KafkaFlow.Admin.Messages
{
    /// <summary>
    /// The message that rewind a consumer to a point in time
    /// </summary>
    [DataContract]
    public class RewindConsumerOffsetToDateTime : IAdminMessage
    {
        /// <summary>
        /// Gets or sets the consumer name that will be rewind
        /// </summary>
        [DataMember(Order = 1)]
        public string ConsumerName { get; set; }

        /// <summary>
        /// Gets or sets the point in time
        /// </summary>
        [DataMember(Order = 2)]
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Gets or sets the topics that will be rewind
        /// </summary>
        [DataMember(Order = 3)]
        public IList<string> Topics { get; set; } = new List<string>();
    }
}
