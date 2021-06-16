namespace KafkaFlow.Admin.WebApi.Contracts
{
    using System.Collections.Generic;

    /// <summary>
    /// The response of the consumers
    /// </summary>
    public class ConsumerResponse
    {
       /// <summary>
       /// Gets or sets the consumer´s name
       /// </summary>
       public string ConsumerName { get; set; }

       /// <summary>
       /// Gets or sets a value indicating whether the consumer is able to be manageable or not
       /// </summary>
       public bool ManagementDisabled { get; set; }

       /// <summary>
       /// Gets or sets the group id
       /// </summary>
       public string GroupId { get; set; }

       /// <summary>
       /// Gets or sets the current number of workers allocated by the consumer
       /// </summary>
       public int WorkersCount { get; set; }

       /// <summary>
       /// Gets or sets the current topics subscription
       /// </summary>
       public IEnumerable<string> Subscription { get; set; }

       /// <summary>
       /// Gets or sets the (dynamic) group member id of this consumer (as set by the broker).
       /// </summary>
       public string MemberId { get; set; }

       /// <summary>
       ///     Gets or sets the name of this client instance.
       ///     Contains (but is not equal to) the client.id configuration parameter.
       /// </summary>
       /// <remarks>
       ///     This name will be unique across all client
       ///     instances in a given application which allows
       ///     log messages to be associated with the
       ///     corresponding instance.
       /// </remarks>
       public string ClientInstanceName { get; set; }

       /// <summary>
       /// Gets or sets the current consumer status
       /// </summary>
       public string Status { get; set; }
    }
}
