using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace KafkaFlow.Admin.WebApi.Contracts
{
    /// <summary>
    /// The response of the consumer group
    /// </summary>
    public class GroupResponse
    {
        /// <summary>
        /// Gets or sets the consumer group id
        /// </summary>
        [Required]
        [JsonProperty(Required = Required.DisallowNull)]
        public string GroupId { get; set; }

        /// <summary>
        /// Gets or sets the consumers collection
        /// </summary>
        public IEnumerable<ConsumerResponse> Consumers { get; set; }
    }
}
