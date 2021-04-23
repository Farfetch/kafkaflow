namespace KafkaFlow.Admin.WebApi.Contracts
{
    using System.Collections.Generic;
    using KafkaFlow.Consumers;

    /// <summary>
    /// The response of the consumer groups
    /// </summary>
    public class GroupResponse
    {
        /// <summary>
        /// Gets or sets the consumer group id
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// Gets or sets the consumers collection
        /// </summary>
        public IEnumerable<IMessageConsumer> Consumers { get; set; }
    }
}
