using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace KafkaFlow.Admin.WebApi.Contracts
{
    /// <summary>
    /// The response of the consumers
    /// </summary>
    public class ConsumerResponse
    {
        /// <summary>
        /// Gets or sets the consumer´s name
        /// </summary>
        [Required]
        [JsonProperty(Required = Required.DisallowNull)]
        public string ConsumerName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the consumer is able to be manageable or not
        /// </summary>
        public bool ManagementDisabled { get; set; }

        /// <summary>
        /// Gets or sets the group id
        /// </summary>
        [Required]
        [JsonProperty(Required = Required.DisallowNull)]
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
        [Required]
        [JsonProperty(Required = Required.DisallowNull)]
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
        [Required]
        [JsonProperty(Required = Required.DisallowNull)]
        public string ClientInstanceName { get; set; }

        /// <summary>
        /// Gets or sets the current consumer status
        /// </summary>
        [Required]
        [JsonProperty(Required = Required.DisallowNull)]
        public string Status { get; set; }
    }
}
