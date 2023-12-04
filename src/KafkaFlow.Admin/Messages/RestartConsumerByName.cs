using System.Runtime.Serialization;

namespace KafkaFlow.Admin.Messages;

/// <summary>
/// The message that destroy and recreates the internal consumer
/// </summary>
[DataContract]
public class RestartConsumerByName : IAdminMessage
{
    /// <summary>
    /// Gets or sets the consumer name
    /// </summary>
    [DataMember(Order = 1)]
    public string ConsumerName { get; set; }
}
