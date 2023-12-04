using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KafkaFlow.Admin.Messages;

/// <summary>
/// The message that resume a paused consumer
/// </summary>
[DataContract]
public class ResumeConsumerByName : IAdminMessage
{
    /// <summary>
    /// Gets or sets the consumer name that will be resumed
    /// </summary>
    [DataMember(Order = 1)]
    public string ConsumerName { get; set; }

    /// <summary>
    /// Gets or sets the topics that will be resumed
    /// </summary>
    [DataMember(Order = 2)]
    public IList<string> Topics { get; set; } = new List<string>();
}
