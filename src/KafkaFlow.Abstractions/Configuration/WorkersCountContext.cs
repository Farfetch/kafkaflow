namespace KafkaFlow.Configuration
{
    using System.Collections.Generic;

    /// <summary>
    /// A metadata class with some context information help to calculate the number of workers
    /// </summary>
    public class WorkersCountContext
    {
        public WorkersCountContext(
            string consumerName,
            string consumerGroupId,
            IReadOnlyCollection<TopicPartitions> assignedTopicsPartitions)
        {
            this.ConsumerName = consumerName;
            this.ConsumerGroupId = consumerGroupId;
            this.AssignedTopicsPartitions = assignedTopicsPartitions;
        }

        public string ConsumerName { get; }

        public string ConsumerGroupId { get; }

        /// <summary>
        /// Gets the assigned partitions to the consumer
        /// </summary>
        public IReadOnlyCollection<TopicPartitions> AssignedTopicsPartitions { get; }
    }
}
