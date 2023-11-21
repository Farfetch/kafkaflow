using System.Collections.Generic;

namespace KafkaFlow.Admin.WebApi.Contracts
{
    /// <summary>
    /// The response of the consumer groups
    /// </summary>
    public class GroupsResponse
    {
        /// <summary>
        /// Gets or sets the groups collection
        /// </summary>
        public IEnumerable<GroupResponse> Groups { get; set; }
    }
}
