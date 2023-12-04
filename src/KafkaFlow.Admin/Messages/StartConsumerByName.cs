using System.Runtime.Serialization;

namespace KafkaFlow.Admin.Messages;

/// <summary>
/// The message that starts a consumer
/// </summary>
[DataContract]
public class StartConsumerByName : IAdminMessage
{
    /// <summary>
    /// Gets or sets the consumer name that will be started
    /// </summary>
    [DataMember(Order = 1)]
    public string ConsumerName { get; set; }
}
