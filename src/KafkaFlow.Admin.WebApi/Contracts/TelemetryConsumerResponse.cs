namespace KafkaFlow.Admin.WebApi.Contracts
{
    using System.Collections.Generic;
    using KafkaFlow.Consumers;

    /// <summary>
    /// The response of the telemetry consumers
    /// </summary>
    public class TelemetryConsumerResponse
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
       /// Gets or sets all the consumer partition assignments (data received from metrics events)
       /// </summary>
       public IEnumerable<PartitionAssignment> PartitionAssignments { get; set; }

       /// <summary>
       /// Gets or sets the current consumer flow status
       /// </summary>
       public string FlowStatus { get; set; }
    }
}
