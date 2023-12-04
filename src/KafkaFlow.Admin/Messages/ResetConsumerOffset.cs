using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KafkaFlow.Admin.Messages;

/// <summary>
/// The message that rewind the offset of all partitions/topics of a consumer to the beginning
/// </summary>
[DataContract]
public class ResetConsumerOffset : IAdminMessage
{
    /// <summary>
    /// Gets or sets the consumer name that will be reset
    /// </summary>
    [DataMember(Order = 1)]
    public string ConsumerName { get; set; }

    /// <summary>
    /// Gets or sets the topics that will be reset
    /// </summary>
    [DataMember(Order = 2)]
    public IList<string> Topics { get; set; } = new List<string>();
}
