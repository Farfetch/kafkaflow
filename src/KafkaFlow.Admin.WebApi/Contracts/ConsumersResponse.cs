using System.Collections.Generic;

namespace KafkaFlow.Admin.WebApi.Contracts;

/// <summary>
/// The response of the consumers
/// </summary>
public class ConsumersResponse
{
    /// <summary>
    /// Gets or sets the consumers collection
    /// </summary>
    public IEnumerable<ConsumerResponse> Consumers { get; set; }
}
