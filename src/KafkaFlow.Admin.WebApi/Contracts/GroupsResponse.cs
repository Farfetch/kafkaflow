namespace KafkaFlow.Admin.WebApi.Contracts
{
    using System.Collections.Generic;

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
